
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

function Navbar() {
  const navigate = useNavigate();

  // Get authentication state and logout function
  const { isAuthenticated, logout, user } = useAuth();

  function handleLogout() {
    logout();
    navigate("/login");
  }

  return (
    <nav>
      <h2>YogaSphere</h2>

      <div>
        <Link to="/">Home</Link>

        {isAuthenticated ? (
          <>
            {user && <span>Welcome, {user.firstName}</span>}

            <button
              type="button"
              className="nav-button"
              onClick={handleLogout}
            >
              Logout
            </button>
          </>
        ) : (
          <>
            <Link to="/login">Login</Link>
            <Link to="/register">Register</Link>
          </>
        )}
      </div>
    </nav>
  );
}

export default Navbar;
