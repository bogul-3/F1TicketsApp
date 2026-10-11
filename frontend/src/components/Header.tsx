import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";
import { useState } from "react";

export default function Header() {
  const navigate = useNavigate();
  const { user, logout } = useAuth();

  // state za dropdown menu
  const [adminOpen, setAdminOpen] = useState(false);

  const handleLogout = () => {
    logout();
    navigate("/");
  };

  return (
    <header className="sticky top-0 w-full bg-black text-white z-50 shadow-md">
      <div className="max-w-6xl mx-auto flex justify-between items-center p-4">
        <Link to="/" className="text-xl font-bold">
          F1 Tickets
        </Link>

        <div className="flex gap-4 items-center">
          {!user && (
            <>
              <Link to="/login" className="hover:text-red-400">
                Login
              </Link>

              <Link to="/register" className="hover:text-red-400">
                Register
              </Link>
            </>
          )}

          {/* CONDITIONAL RENDERING */}
          {user && (
            <>
              {/* Ime korisnika */}
              <span className="font-semibold">{user.name}</span>

              {/* Link ka user tiketima */}
              <Link
                to="/my-tickets"
                className="hover:text-red-400 font-semibold"
              >
                My Tickets
              </Link>

              {/* Admin dropdown */}
              {user.role === "ADMIN" && (
                <div className="relative">
                  <button
                    onClick={() => setAdminOpen(!adminOpen)}
                    className="hover:text-red-400 font-semibold"
                  >
                    Admin ▼
                  </button>

                  {adminOpen && (
                    <div className="absolute right-0 mt-2 w-40 bg-white text-black rounded shadow-lg">
                      <Link
                        to="/admin"
                        className="block px-4 py-2 hover:bg-gray-200"
                        onClick={() => setAdminOpen(false)}
                      >
                        Race Events
                      </Link>
                      {/* Dodaj još admin linkova ovde po potrebi */}
                    </div>
                  )}
                </div>
              )}

              <button
                onClick={handleLogout}
                className="hover:text-red-400 font-semibold"
              >
                Logout
              </button>
            </>
          )}
        </div>
      </div>
    </header>
  );
}