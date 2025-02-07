const baseUrl = 'http://localhost:5139';

export default class backendApi {
  static async getBranches(): Promise<string[]> {
    const response = await fetch(`${baseUrl}/api/branches`);
    if (!response.ok) {
      throw new Error('Failed to fetch branches');
    }
    return response.json();
  }

  static async getTopSellers(branch: string): Promise<any[]> {
    const response = await fetch(`${baseUrl}/api/top-sellers?branch=${branch}`);
    if (!response.ok) {
      throw new Error('Failed to fetch top sellers');
    }
    return response.json();
  }
}