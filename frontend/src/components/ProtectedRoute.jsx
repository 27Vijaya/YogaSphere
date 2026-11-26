import { Navigate } from "react-router-dom";
import { useAuth } from "../context/useAuth";

function ProtectedRoute({ children, allowedRoles }) {
  const { isAuthenticated, user } = useAuth();

  // Redirect unauthenticated users to login.
  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  // Determine the correct dashboard for the logged-in user's role.
  const dashboardPath =
    user?.role === "Trainer"
      ? "/trainer"
      : user?.role === "Admin"
        ? "/admin"
        : "/dashboard";

  // Redirect users who do not have permission to access this route.
  if (allowedRoles && (!user || !allowedRoles.includes(user.role))) {
    // Avoid redirecting to the same restricted route.
    if (window.location.pathname === dashboardPath) {
      return <Navigate to="/" replace />;
    }

    return <Navigate to={dashboardPath} replace />;
  }

  return children;
}

export default ProtectedRoute;
