import { Navigate } from "react-router-dom";
import { useAuth } from "../context/useAuth";

function PublicRoute({ children }) {
  // Get authentication status from AuthContext
  const { isAuthenticated } = useAuth();

  // Redirect logged-in users to Home
  if (isAuthenticated) {
    return <Navigate to="/" replace />;
  }

  return children;
}

export default PublicRoute;
