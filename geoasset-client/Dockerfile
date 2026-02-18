# Build stage
FROM node:22-alpine AS build
WORKDIR /app

# API URL for browser to call backend (passed at build time)
ARG VITE_API_URL=http://localhost:5140
ENV VITE_API_URL=$VITE_API_URL

COPY package.json package-lock.json* ./
RUN npm ci

COPY . .
RUN npm run build

# Serve with nginx
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

# Remove default static assets
RUN rm -rf ./*

COPY --from=build /app/dist .

# Default nginx config serves from / and handles SPA fallback
RUN echo 'server { listen 80; root /usr/share/nginx/html; index index.html; location / { try_files $uri $uri/ /index.html; } }' > /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
