import { Link } from "react-router-dom";
import { useAuth } from "../contexts/AuthContext";

export default function LandingPage() {
  const { isAuthenticated } = useAuth();

  return (
    <div className="flex flex-col items-center justify-center gap-8 py-20">
      <div className="text-center">
        <h1 className="text-4xl font-bold text-gray-800 mb-4">OPTODEV Member Portal</h1>
        <p className="text-lg text-gray-600 max-w-md">
          Register as an OPTODEV member or log in to manage member records.
        </p>
      </div>

      <div className="flex gap-4">
        <Link
          to="/register"
          className="bg-blue-600 text-white px-8 py-3 rounded-lg font-semibold hover:bg-blue-700"
        >
          Register as Member
        </Link>

        {isAuthenticated ? (
          <Link
            to="/admin/members"
            className="bg-gray-700 text-white px-8 py-3 rounded-lg font-semibold hover:bg-gray-800"
          >
            Admin Dashboard
          </Link>
        ) : (
          <Link
            to="/admin/login"
            className="bg-gray-700 text-white px-8 py-3 rounded-lg font-semibold hover:bg-gray-800"
          >
            Admin Login
          </Link>
        )}
      </div>
    </div>
  );
}
