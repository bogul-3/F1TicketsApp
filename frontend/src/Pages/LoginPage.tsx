import { useState } from "react";
import { useNavigate } from "react-router-dom";
import AuthFormInput from "../components/AuthFormInput";
import { useAuth } from "../context/AuthContext";
import { loginUser } from "../services/authService";
import { saveToken } from "../utils/auth";

export default function LoginPage() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");

  const { login } = useAuth();
  const navigate = useNavigate();

  // Type guard za role
  function isValidRole(role: string): role is "ADMIN" | "USER" {
    return role === "ADMIN" || role === "USER";
  }

  const handleLogin = async () => {
    if (!email || !password) {
      setErrorMessage("Molimo unesite email i lozinku");
      return;
    }

    setIsLoading(true);
    setErrorMessage("");

    try {
      const res = await loginUser({ email, password });

      // Sačuvaj token
      saveToken(res.token);

      // Type-safe role
      const userRole = isValidRole(res.role) ? res.role : "USER";

      // Update auth context
      login(
        {
          id: res.userId, // <-- userId iz backend-a
          name: res.name,
          role: userRole,
          email: res.email
        },
        res.token,
      );

      // Redirect na homepage
      navigate("/");
    } catch (err: any) {
      console.error("Login failed:", err);
      setErrorMessage(err.message || "Login nije uspeo");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="flex items-center justify-center min-h-screen bg-gray-100">
      <div className="bg-white p-8 rounded-xl shadow-lg w-[400px] flex flex-col gap-6">
        <h1 className="text-3xl font-bold text-center text-gray-800">Login</h1>

        {/* Prikaz greške */}
        {errorMessage && (
          <p className="text-red-500 text-sm text-center">{errorMessage}</p>
        )}

        <AuthFormInput
          label="Email"
          value={email}
          onChange={setEmail}
          placeholder="example@test.com"
        />

        <AuthFormInput
          label="Password"
          type="password"
          value={password}
          onChange={setPassword}
          placeholder="••••••••"
        />

        <button
          onClick={handleLogin}
          disabled={isLoading}
          className={`bg-red-600 text-white p-3 rounded-lg hover:bg-red-700 active:scale-95 transition transform font-semibold ${
            isLoading ? "opacity-70 cursor-not-allowed" : ""
          }`}
        >
          {isLoading ? "Logging in..." : "Login"}
        </button>
      </div>
    </div>
  );
}
