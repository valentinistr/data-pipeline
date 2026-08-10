/**
 * API base URL for HttpClient calls.
 * Uses the Angular dev-server proxy (`proxy.conf.json`) so the browser talks to the
 * same origin as the client. That works in Codespaces/devcontainers where
 * `localhost:5133` in the browser is not the container's API.
 */
export const API_BASE_URL = '/api';
