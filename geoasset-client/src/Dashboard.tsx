import './Dashboard.css';

export interface InspectionLog {
  id: number;
  assetId: number;
  inspectionDate: string;
  inspectorName: string;
  notes: string;
}

export interface Asset {
  id: number;
  name: string;
  type: string;
  latitude: number;
  longitude: number;
  status: string;
  lastUpdated?: string;
  inspectionLogs?: InspectionLog[];
}

const STALE_DAYS = 30;

function isStale(lastUpdated: string | undefined): boolean {
  if (!lastUpdated) return true;
  const updated = new Date(lastUpdated).getTime();
  const cutoff = Date.now() - STALE_DAYS * 24 * 60 * 60 * 1000;
  return updated < cutoff;
}

interface DashboardProps {
  assets: Asset[];
}

export function Dashboard({ assets }: DashboardProps) {
  const totalAssets = assets.length;
  const activeAssets = assets.filter((a) => a.status === 'Active').length;
  const needsInspection = assets.filter((a) => isStale(a.lastUpdated)).length;
  const totalInspections = assets.reduce(
    (sum, a) => sum + (a.inspectionLogs?.length ?? 0),
    0
  );

  return (
    <header className="dashboard-bar" role="banner">
      <div className="dashboard-cards">
        <div className="dashboard-card">
          <span className="dashboard-card-label">Total Assets</span>
          <span className="dashboard-card-value">{totalAssets}</span>
        </div>
        <div className="dashboard-card">
          <span className="dashboard-card-label">Active Assets</span>
          <span className="dashboard-card-value">{activeAssets}</span>
        </div>
        <div
          className={`dashboard-card dashboard-card--needs-inspection ${needsInspection > 0 ? 'dashboard-card--alert' : ''}`}
        >
          <span className="dashboard-card-label">
            Needs Inspection
            {needsInspection > 0 && (
              <span className="dashboard-card-pulse" aria-hidden>
                ●
              </span>
            )}
          </span>
          <span className="dashboard-card-value">{needsInspection}</span>
        </div>
        <div className="dashboard-card">
          <span className="dashboard-card-label">Total Inspections</span>
          <span className="dashboard-card-value">{totalInspections}</span>
        </div>
      </div>
    </header>
  );
}
