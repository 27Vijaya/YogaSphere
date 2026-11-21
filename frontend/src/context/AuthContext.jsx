
import { createContext, useContext, useState } from "react";

// Create the authentication context
const AuthContext = createContext(null);

// Provide authentication information to the application
export function AuthProvider({ children }) {
  // Read the existing token when the application starts
  const [token, setToken] = useState(
    () => localStorage.getItem("accessToken")
  );

  const [user, setUser] = useState(null);

  // Save login information
  function login(accessToken, userData) {
    localStorage.setItem("accessToken", accessToken);
    setToken(accessToken);
    setUser(userData);
  }

  // Clear login information
  function logout() {
    localStorage.removeItem("accessToken");
    setToken(null);
    setUser(null);
  }

  return (
    <AuthContext.Provider
      value={{
        token,
        user,
        isAuthenticated: Boolean(token),
        login,
        logout,
        setUser,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
}

// Custom hook to access authentication information
export function useAuth() {
  return useContext(AuthContext);
}
