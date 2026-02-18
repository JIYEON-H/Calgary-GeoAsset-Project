import { useEffect, useRef, useState } from 'react';
import { MapContainer, TileLayer, Marker, Popup, useMap } from 'react-leaflet';
import L from 'leaflet';
import axios from 'axios';
import 'leaflet/dist/leaflet.css';
import './App.css';
import { Dashboard, type Asset, type InspectionLog } from './Dashboard';

const API_BASE = import.meta.env.VITE_API_URL ?? 'http://localhost:5140';

function getTodayDateString(): string {
  return new Date().toISOString().slice(0, 10);
}

const STALE_DAYS = 30;

function isStale(lastUpdated: string | undefined): boolean {
  if (!lastUpdated) return true;
  const updated = new Date(lastUpdated).getTime();
  const cutoff = Date.now() - STALE_DAYS * 24 * 60 * 60 * 1000;
  return updated < cutoff;
}

function getStatusColor(status: string): string {
  switch (status) {
    case 'Active':
      return '#2ecc71';
    case 'Maintenance':
      return '#f39c12';
    case 'Offline':
      return '#e74c3c';
    default:
      return '#3498db';
  }
}

function createCustomIcon(status: string, lastUpdated?: string): L.DivIcon {
  const color = getStatusColor(status);
  const needsInspection = isStale(lastUpdated);
  return L.divIcon({
    className: 'custom-marker',
    html: `<div class="marker-dot ${needsInspection ? 'needs-inspection' : ''}" style="
      width: 15px;
      height: 15px;
      background-color: ${color};
      border: ${needsInspection ? '3px' : '2px'} solid ${needsInspection ? '#f1c40f' : 'white'};
      border-radius: 50%;
      box-shadow: 0 1px 3px rgba(0,0,0,0.3);
    "></div>`,
    iconSize: [15, 15],
    iconAnchor: [7.5, 7.5],
  });
}

function MapController({ mapRef }: { mapRef: React.MutableRefObject<L.Map | null> }) {
  const map = useMap();
  useEffect(() => {
    mapRef.current = map;
    return () => {
      mapRef.current = null;
    };
  }, [map, mapRef]);
  return null;
}

type StatusFilterValue = 'All' | 'Active' | 'Maintenance' | 'Offline';

function App() {
  const [assets, setAssets] = useState<Asset[]>([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState<StatusFilterValue>('All');
  const [showStaleOnly, setShowStaleOnly] = useState(false);
  const [selectedAssetId, setSelectedAssetId] = useState<number | null>(null);
  const [addLogForm, setAddLogForm] = useState({
    inspectorName: '',
    notes: '',
    inspectionDate: getTodayDateString(),
  });
  const [addLogSubmitting, setAddLogSubmitting] = useState(false);
  const [addLogError, setAddLogError] = useState<string | null>(null);
  const mapRef = useRef<L.Map | null>(null);

  const selectedAsset = selectedAssetId != null
    ? assets.find((a) => a.id === selectedAssetId) ?? null
    : null;

  const filteredAssets = assets.filter((asset) => {
    const matchesSearch = asset.name
      .toLowerCase()
      .includes(searchTerm.trim().toLowerCase());
    const matchesStatus =
      statusFilter === 'All' || asset.status === statusFilter;
    const matchesStale = !showStaleOnly || isStale(asset.lastUpdated);
    return matchesSearch && matchesStatus && matchesStale;
  });

  useEffect(() => {
    const fetchAssets = async () => {
      try {
        const response = await axios.get<Asset[]>(
          `${API_BASE}/api/assets?includeInspectionLogs=true`
        );
        setAssets(response.data);
      } catch (error) {
        console.error("Data loading failed:", error);
      }
    };
    fetchAssets();
  }, []);

  const handleSelectAsset = (asset: Asset) => {
    setSelectedAssetId(asset.id);
    mapRef.current?.flyTo([asset.latitude, asset.longitude], 15);
  };

  const handleAddLogSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (selectedAsset == null) return;
    setAddLogError(null);
    setAddLogSubmitting(true);
    try {
      const payload = {
        assetId: selectedAsset.id,
        inspectorName: addLogForm.inspectorName.trim(),
        notes: addLogForm.notes.trim(),
        inspectionDate: addLogForm.inspectionDate,
      };
      const response = await axios.post<InspectionLog>(
        `${API_BASE}/api/InspectionLogs`,
        payload
      );
      const newLog = response.data;
      setAssets((prev) =>
        prev.map((a) =>
          a.id === selectedAsset.id
            ? { ...a, inspectionLogs: [...(a.inspectionLogs ?? []), newLog] }
            : a
        )
      );
      setAddLogForm({
        inspectorName: '',
        notes: '',
        inspectionDate: getTodayDateString(),
      });
    } catch (err) {
      const message = axios.isAxiosError(err) && err.response?.data?.message
        ? String(err.response.data.message)
        : 'Failed to add inspection log.';
      setAddLogError(message);
    } finally {
      setAddLogSubmitting(false);
    }
  };

  return (
    <div className="app-layout">
      <Dashboard assets={assets} />
      <div className="app-main">
      <aside className="sidebar">
        <div className="sidebar-header">
          <h2 className="sidebar-title">GeoAsset Connect</h2>
          <input
            type="search"
            className="sidebar-search"
            placeholder="Search by name..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            aria-label="Search assets"
          />
          <div className="sidebar-filters">
            <label className="filter-label">Status</label>
            <select
              className="sidebar-select"
              value={statusFilter}
              onChange={(e) => setStatusFilter(e.target.value as StatusFilterValue)}
              aria-label="Filter by status"
            >
              <option value="All">All</option>
              <option value="Active">Active</option>
              <option value="Maintenance">Maintenance</option>
              <option value="Offline">Offline</option>
            </select>
            <label className={`stale-only-toggle ${showStaleOnly ? 'active' : ''}`}>
              <input
                type="checkbox"
                checked={showStaleOnly}
                onChange={(e) => setShowStaleOnly(e.target.checked)}
                aria-label="Show needs inspection only"
              />
              <span className="stale-only-label">Show Needs Inspection Only</span>
            </label>
          </div>
        </div>
        <ul className="asset-list">
          {filteredAssets.length === 0 ? (
            <li className="asset-list-empty">No assets found</li>
          ) : (
          filteredAssets.map((asset) => {
            const stale = isStale(asset.lastUpdated);
            return (
              <li
                key={asset.id}
                className={`asset-list-item ${stale ? 'stale' : ''} ${selectedAssetId === asset.id ? 'selected' : ''}`}
                onClick={() => handleSelectAsset(asset)}
                role="button"
                tabIndex={0}
                onKeyDown={(e) => {
                  if (e.key === 'Enter' || e.key === ' ') {
                    e.preventDefault();
                    handleSelectAsset(asset);
                  }
                }}
              >
                <span className="asset-name">
                  {stale && <span className="stale-icon" aria-hidden>⚠️</span>}
                  {asset.name}
                </span>
                <span className="asset-status" style={{ color: getStatusColor(asset.status) }}>
                  {asset.status}
                </span>
              </li>
            );
          })
          )}
        </ul>
      </aside>
      <div className="map-wrapper">
        <MapContainer
          center={[51.0447, -114.0719]}
          zoom={11}
          style={{ height: '100%', width: '100%' }}
        >
          <MapController mapRef={mapRef} />
          <TileLayer
            attribution='&copy; OpenStreetMap'
            url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
          />
          {filteredAssets.map((asset) => (
            <Marker
              key={asset.id}
              position={[asset.latitude, asset.longitude]}
              icon={createCustomIcon(asset.status, asset.lastUpdated)}
              eventHandlers={{
                click: () => setSelectedAssetId(asset.id),
              }}
            >
              <Popup>
                <strong>{asset.name}</strong><br />
                Status: {asset.status}
                {asset.lastUpdated && (
                  <>
                    <br />
                    Last updated: {new Date(asset.lastUpdated).toLocaleDateString()}
                  </>
                )}
              </Popup>
            </Marker>
          ))}
        </MapContainer>
      </div>
      <div
        className={`detail-panel-wrapper ${selectedAsset != null ? 'detail-panel-wrapper--open' : ''}`}
        aria-hidden={selectedAsset == null}
      >
        {selectedAsset != null && (
          <aside className="detail-panel" aria-label="Asset detail">
            <div className="detail-panel-header">
              <h2 className="detail-panel-title">{selectedAsset.name}</h2>
              <button
                type="button"
                className="detail-panel-close"
                onClick={() => setSelectedAssetId(null)}
                aria-label="Close detail panel"
              >
                ×
              </button>
            </div>
            <div className="detail-panel-body">
              <div className="detail-panel-meta">
                <span className="detail-panel-status" style={{ color: getStatusColor(selectedAsset.status) }}>
                  {selectedAsset.status}
                </span>
                <span className="detail-panel-type">{selectedAsset.type}</span>
                {selectedAsset.lastUpdated && (
                  <p className="detail-panel-updated">
                    Last updated: {new Date(selectedAsset.lastUpdated).toLocaleDateString()}
                  </p>
                )}
              </div>
              <section className="detail-panel-inspection" aria-labelledby="detail-inspection-heading">
                <h3 id="detail-inspection-heading" className="detail-panel-inspection-title">
                  Inspection Log
                </h3>
                <div className="detail-panel-inspection-list">
                  {!selectedAsset.inspectionLogs?.length ? (
                    <p className="detail-panel-inspection-empty">No inspection history available.</p>
                  ) : (
                    <ul className="inspection-timeline">
                      {[...(selectedAsset.inspectionLogs ?? [])]
                        .sort((a, b) => new Date(b.inspectionDate).getTime() - new Date(a.inspectionDate).getTime())
                        .map((log) => (
                          <li key={log.id} className="inspection-log-item">
                            <div className="inspection-log-card">
                              <time className="inspection-log-date" dateTime={log.inspectionDate}>
                                {new Date(log.inspectionDate).toLocaleDateString(undefined, {
                                  year: 'numeric',
                                  month: 'short',
                                  day: 'numeric',
                                })}
                              </time>
                              <span className="inspection-log-inspector">{log.inspectorName}</span>
                              {log.notes && <p className="inspection-log-notes">{log.notes}</p>}
                            </div>
                          </li>
                        ))}
                    </ul>
                  )}
                </div>
              </section>
              <form
                className="quick-add-log"
                onSubmit={handleAddLogSubmit}
                aria-labelledby="quick-add-log-heading"
              >
                <h3 id="quick-add-log-heading" className="quick-add-log-title">
                  Quick Add Log
                </h3>
                <label className="quick-add-log-label">
                  Inspector Name
                  <input
                    type="text"
                    className="quick-add-log-input"
                    value={addLogForm.inspectorName}
                    onChange={(e) =>
                      setAddLogForm((f) => ({ ...f, inspectorName: e.target.value }))
                    }
                    placeholder="Name"
                    disabled={addLogSubmitting}
                    autoComplete="name"
                  />
                </label>
                <label className="quick-add-log-label">
                  Notes
                  <textarea
                    className="quick-add-log-textarea"
                    value={addLogForm.notes}
                    onChange={(e) =>
                      setAddLogForm((f) => ({ ...f, notes: e.target.value }))
                    }
                    placeholder="Optional notes"
                    rows={2}
                    disabled={addLogSubmitting}
                  />
                </label>
                <label className="quick-add-log-label">
                  Inspection Date
                  <input
                    type="date"
                    className="quick-add-log-input"
                    value={addLogForm.inspectionDate}
                    onChange={(e) =>
                      setAddLogForm((f) => ({ ...f, inspectionDate: e.target.value }))
                    }
                    disabled={addLogSubmitting}
                  />
                </label>
                {addLogError != null && (
                  <p className="quick-add-log-error" role="alert">
                    {addLogError}
                  </p>
                )}
                <button
                  type="submit"
                  className="quick-add-log-submit"
                  disabled={addLogSubmitting}
                >
                  {addLogSubmitting ? 'Adding…' : 'Add Log'}
                </button>
              </form>
            </div>
          </aside>
        )}
      </div>
      </div>
    </div>
  );
}

export default App;