import { createContext, useContext, useState } from "react";

// Za LocalStorage
export type User = {
  id: string;
  name: string;
  email: string; 
  role: "ADMIN" | "USER";
};

// Sta kontekst sadrzi
type AuthContextType = {
  user: User | null;
  login: (user: User, token: string) => void;
  logout: () => void;
};

const AuthContext = createContext<AuthContextType | undefined>(undefined);

// Komponenta koja cuva state korisnika
export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<User | null>(() => {
    const storedUser = localStorage.getItem("user");
    return storedUser ? JSON.parse(storedUser) : null;
  });

  // Login funkcija
  const login = (userData: User, token: string) => {
    const formattedUser: User = {
      id: userData.id,
      name: userData.name,
      email: userData.email, 
      role: userData.role,
    };

    localStorage.setItem("token", token);
    localStorage.setItem("user", JSON.stringify(formattedUser));
    setUser(formattedUser);
  };

  // Logout funkcija
  const logout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
    setUser(null);
  };

  // AuthContext cini user, login i logout dostunim svim komponentama (children)
  return (
    <AuthContext.Provider value={{ user, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

// Custom hook da moze ovako da se pise const { user, login, logout } = useAuth();
export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) throw new Error("useAuth must be used inside AuthProvider");
  return context;
}