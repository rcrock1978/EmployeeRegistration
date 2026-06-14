const API_BASE = import.meta.env.VITE_API_BASE ?? "http://localhost:5000";

export async function registerMember(
  data: unknown
): Promise<Response> {
  return fetch(`${API_BASE}/api/members`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
  });
}
