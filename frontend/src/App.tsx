import { BrowserRouter, Route, Routes } from "react-router-dom";
import Home from "./Pages/Home";
import Booking from "./Pages/Booking";
import RegisterPage from "./Pages/RegisterPage";
import LoginPage from "./Pages/LoginPage";
import Header from "./components/Header";
import ProtectedRoute from "./components/ProtectedRoute";
import AdminPage from "./Pages/admin/AdminPage";
import UserTicketsPage from "./Pages/UserTicketsPage";

export default function App() {
  return (
    <BrowserRouter>
      <div className="min-h-screen flex flex-col">
        <Header />

        <main className="max-w-6xl mx-auto w-full p-6">
          <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/booking/:id" element={<Booking />} />
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
            <Route path="/my-tickets" element={<UserTicketsPage />} />

            <Route
              path="/admin"
              element={
                <ProtectedRoute role="ADMIN">
                  <AdminPage />
                </ProtectedRoute>
              }
            />
          </Routes>
        </main>
      </div>
    </BrowserRouter>
  );
}