import { useState } from "react";
import { Link, NavLink, useNavigate } from "react-router-dom";
import { useAuth } from "../context/useAuth";
import "./Navbar.css";

function Navbar() {
  const navigate = useNavigate();
  const { isAuthenticated, logout, user } = useAuth();
  const [menuOpen, setMenuOpen] = useState(false);

  // Select the dashboard according to the logged-in user's role.
  const dashboardPath =
    user?.role === "Trainer"
      ? "/trainer"
      : user?.role === "Admin"
        ? "/admin"
        : "/dashboard";

  function closeMenu() {
    setMenuOpen(false);
  }

  function handleLogout() {
    logout();
    closeMenu();
    navigate("/login");
  }

  return (
    <header className="app-header">
      <div className="navbar">
        <Link to="/" className="navbar-brand" onClick={closeMenu}>
          YogaSphere
        </Link>

        {/* Mobile hamburger button */}
        <button
          type="button"
          className="navbar-menu-toggle"
          aria-label={menuOpen ? "Close navigation menu" : "Open navigation menu"}
          aria-expanded={menuOpen}
          onClick={() => setMenuOpen((open) => !open)}
        >
          {menuOpen ? "✕" : "☰"}
        </button>

        <nav className={`navbar-links ${menuOpen ? "menu-open" : ""}`}>
          <Link to="/" onClick={closeMenu}>
            Home
          </Link>

          {isAuthenticated ? (
            <>
              <NavLink to={dashboardPath} onClick={closeMenu}>
                Dashboard
              </NavLink>

              {user && (
                <span className="navbar-welcome">
                  Welcome, {user.firstName}
                </span>
              )}

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
              <Link to="/login" onClick={closeMenu}>Login</Link>
              <Link to="/register" onClick={closeMenu}>Register</Link>
            </>
          )}
        </nav>
      </div>
    </header>
  );
}

export default Navbar;
