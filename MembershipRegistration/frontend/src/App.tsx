import { Routes, Route, Link } from "react-router-dom";
import { AuthProvider, useAuth } from "./contexts/AuthContext";
import LandingPage from "./components/LandingPage";
import LoginPage from "./components/LoginPage";
import RegistrationWizard from "./components/RegistrationWizard";
import AdminMemberList from "./components/AdminMemberList";
import AdminMemberDetail from "./components/AdminMemberDetail";
import ProtectedRoute from "./components/ProtectedRoute";

function Header() {
  const { isAuthenticated, role, logout } = useAuth();

  return (
    <header className="bg-white shadow-sm border-b">
      <div className="max-w-7xl mx-auto px-4 py-4 flex justify-between items-center">
        <Link to="/" className="text-xl font-bold text-gray-800">OPTODEV</Link>
        {isAuthenticated && (
          <div className="flex items-center gap-4">
            <Link to="/admin/members" className="text-sm text-blue-600 hover:underline">Members</Link>
            <span className="text-sm text-gray-500">{role}</span>
            <button onClick={logout} className="text-sm text-red-600 hover:underline">Logout</button>
          </div>
        )}
      </div>
    </header>
  );
}

function AppLayout() {
  return (
    <AuthProvider>
      <div className="min-h-screen bg-gray-50">
        <Header />
        <main className="max-w-7xl mx-auto px-4 py-8">
          <Routes>
            <Route path="/" element={<LandingPage />} />
            <Route path="/register" element={<RegistrationWizard />} />
            <Route path="/admin/login" element={<LoginPage />} />
            <Route
              path="/admin/members"
              element={
                <ProtectedRoute>
                  <AdminMemberList />
                </ProtectedRoute>
              }
            />
            <Route
              path="/admin/members/:id"
              element={
                <ProtectedRoute>
                  <AdminMemberDetail />
                </ProtectedRoute>
              }
            />
          </Routes>
        </main>
      </div>
    </AuthProvider>
  );
}

export default function App() {
  return <AppLayout />;
}
