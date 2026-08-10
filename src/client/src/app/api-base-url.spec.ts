import { API_BASE_URL } from './api-base-url';

describe('API_BASE_URL', () => {
  it('uses a same-origin /api prefix so the Angular proxy can reach the .NET API', () => {
    expect(API_BASE_URL).toBe('/api');
  });
});
