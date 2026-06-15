import { useEffect, useState, useCallback } from "react";
import { useNavigate } from "react-router-dom";
import { listMembers } from "../lib/api";
import { useAuth } from "../contexts/AuthContext";
import type { MemberListItem } from "../types/api";

interface Filters {
  lastName: string;
  email: string;
  employeeLevel: string;
}

export default function AdminMemberList() {
  const { logout, role } = useAuth();
  const navigate = useNavigate();
  const [members, setMembers] = useState<MemberListItem[]>([]);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [filters, setFilters] = useState<Filters>({ lastName: "", email: "", employeeLevel: "" });
  const [appliedFilters, setAppliedFilters] = useState<Filters>({ lastName: "", email: "", employeeLevel: "" });

  const fetchMembers = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const res = await listMembers({
        page,
        pageSize: 20,
        lastName: appliedFilters.lastName || undefined,
        email: appliedFilters.email || undefined,
        employeeLevel: appliedFilters.employeeLevel || undefined,
      });
      if (res.isSuccess && res.value) {
        setMembers(res.value.items);
        setTotalPages(res.value.totalPages);
        setTotalCount(res.value.totalCount);
      } else {
        setError("Failed to load members.");
      }
    } catch {
      setError("Network error. Please try again.");
    } finally {
      setLoading(false);
    }
  }, [page, appliedFilters]);

  useEffect(() => {
    fetchMembers();
  }, [fetchMembers]);

  function handleSearch(e: React.FormEvent) {
    e.preventDefault();
    setPage(1);
    setAppliedFilters({ ...filters });
  }

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold text-gray-800">Members</h1>
        <div className="flex items-center gap-4">
          <span className="text-sm text-gray-500">{role}</span>
          <button onClick={logout} className="text-sm text-red-600 hover:underline">Logout</button>
        </div>
      </div>

      <form onSubmit={handleSearch} className="flex gap-3 mb-6">
        <input
          placeholder="Last name"
          value={filters.lastName}
          onChange={(e) => setFilters({ ...filters, lastName: e.target.value })}
          className="border rounded p-2 flex-1"
        />
        <input
          placeholder="Email"
          value={filters.email}
          onChange={(e) => setFilters({ ...filters, email: e.target.value })}
          className="border rounded p-2 flex-1"
        />
        <select
          value={filters.employeeLevel}
          onChange={(e) => setFilters({ ...filters, employeeLevel: e.target.value })}
          className="border rounded p-2"
        >
          <option value="">All Levels</option>
          <option value="PTS">PTS</option>
          <option value="RNF">RNF</option>
        </select>
        <button type="submit" className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700">
          Search
        </button>
      </form>

      {error && <p className="text-red-700 mb-4">{error}</p>}

      {loading ? (
        <p className="text-gray-500">Loading...</p>
      ) : members.length === 0 ? (
        <p className="text-gray-500">No members found.</p>
      ) : (
        <>
          <div className="overflow-x-auto">
            <table className="w-full border-collapse">
              <thead>
                <tr className="bg-gray-100">
                  <th className="text-left p-3 border-b cursor-pointer hover:bg-gray-200">Name</th>
                  <th className="text-left p-3 border-b cursor-pointer hover:bg-gray-200">Email</th>
                  <th className="text-left p-3 border-b cursor-pointer hover:bg-gray-200">Status</th>
                  <th className="text-left p-3 border-b cursor-pointer hover:bg-gray-200">Level</th>
                  <th className="text-left p-3 border-b cursor-pointer hover:bg-gray-200">Created</th>
                </tr>
              </thead>
              <tbody>
                {members.map((m) => (
                  <tr
                    key={m.id}
                    className="border-b hover:bg-blue-50 cursor-pointer"
                    onClick={() => navigate(`/admin/members/${m.id}`)}
                  >
                    <td className="p-3">{m.lastName}, {m.firstName}{m.middleName ? ` ${m.middleName}` : ""}</td>
                    <td className="p-3">{m.emailAddress}</td>
                    <td className="p-3">{m.status}</td>
                    <td className="p-3">{m.employeeLevel}</td>
                    <td className="p-3">{new Date(m.createdOn).toLocaleDateString()}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          <div className="flex justify-between items-center mt-4">
            <p className="text-sm text-gray-500">{totalCount} total members</p>
            <div className="flex gap-2 items-center">
              <button
                disabled={page <= 1}
                onClick={() => setPage(page - 1)}
                className="px-3 py-1 border rounded disabled:opacity-50"
              >
                Previous
              </button>
              {Array.from({ length: Math.min(totalPages, 10) }, (_, i) => {
                const start = Math.max(1, page - 4);
                const p = start + i;
                if (p > totalPages) return null;
                return (
                  <button
                    key={p}
                    onClick={() => setPage(p)}
                    className={`px-3 py-1 border rounded ${p === page ? "bg-blue-600 text-white" : "hover:bg-gray-100"}`}
                  >
                    {p}
                  </button>
                );
              })}
              <button
                disabled={page >= totalPages}
                onClick={() => setPage(page + 1)}
                className="px-3 py-1 border rounded disabled:opacity-50"
              >
                Next
              </button>
            </div>
          </div>
        </>
      )}
    </div>
  );
}
