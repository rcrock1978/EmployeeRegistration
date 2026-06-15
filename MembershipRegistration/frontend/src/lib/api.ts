import type { Envelope, LoginResponse, MemberListItem, MemberDetail, PagedResult } from "../types/api";

const API_BASE = import.meta.env.VITE_API_BASE ?? "http://localhost:5000";

export async function registerMember(data: unknown): Promise<Response> {
  return fetch(`${API_BASE}/api/members`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(data),
  });
}

export async function login(email: string, password: string): Promise<Envelope<LoginResponse>> {
  const res = await fetch(`${API_BASE}/api/auth/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ email, password }),
  });
  return res.json();
}

function getToken(): string | null {
  return localStorage.getItem("auth_token");
}

export async function fetchWithAuth(url: string, options?: RequestInit): Promise<Response> {
  const token = getToken();
  const headers: Record<string, string> = {
    "Content-Type": "application/json",
    ...(options?.headers as Record<string, string>),
  };
  if (token) {
    headers["Authorization"] = `Bearer ${token}`;
  }
  const res = await fetch(`${API_BASE}${url}`, { ...options, headers });
  if (res.status === 401) {
    localStorage.removeItem("auth_token");
    localStorage.removeItem("auth_email");
    localStorage.removeItem("auth_role");
    window.location.href = "/admin/login";
  }
  return res;
}

export async function listMembers(params: {
  page?: number;
  pageSize?: number;
  lastName?: string;
  email?: string;
  employeeLevel?: string;
  createdDateFrom?: string;
  createdDateTo?: string;
}): Promise<Envelope<PagedResult<MemberListItem>>> {
  const searchParams = new URLSearchParams();
  if (params.page) searchParams.set("page", String(params.page));
  if (params.pageSize) searchParams.set("pageSize", String(params.pageSize));
  if (params.lastName) searchParams.set("lastName", params.lastName);
  if (params.email) searchParams.set("email", params.email);
  if (params.employeeLevel) searchParams.set("employeeLevel", params.employeeLevel);
  if (params.createdDateFrom) searchParams.set("createdDateFrom", params.createdDateFrom);
  if (params.createdDateTo) searchParams.set("createdDateTo", params.createdDateTo);
  const qs = searchParams.toString();
  const res = await fetchWithAuth(`/api/members${qs ? `?${qs}` : ""}`);
  return res.json();
}

export async function getMemberById(id: string): Promise<Envelope<MemberDetail>> {
  const res = await fetchWithAuth(`/api/members/${id}`);
  return res.json();
}
