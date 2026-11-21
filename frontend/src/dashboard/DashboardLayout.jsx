
import { NavLink, Outlet } from "react-router-dom";
import "./DashboardLayout.css";

function DashboardLayout() {
  return (
    <div className="dashboard-layout">
      {/* Dashboard sidebar */}
      <aside className="dashboard-sidebar">
        <h2>YogaSphere</h2>
        <p className="sidebar-subtitle">Wellness Dashboard</p>

        <nav className="dashboard-nav">
          <NavLink to="/dashboard" end>
            Overview
          </NavLink>

          <NavLink to="/dashboard/profile">
            My Profile
          </NavLink>

          <NavLink to="/dashboard/program">
            Yoga Programs
          </NavLink>

          <NavLink to="/dashboard/classes">
            Yoga Classes
          </NavLink>

          <NavLink to="/dashboard/bookings">
            My Bookings
          </NavLink>
        </nav>
      </aside>

      {/* Dashboard content */}
      <main className="dashboard-content">
        <header className="dashboard-header">
          <h1>Dashboard</h1>
          <p>Manage your YogaSphere activities</p>
        </header>

        <section className="dashboard-page">
          <Outlet />
        </section>
      </main>
    </div>
  );
}

export default DashboardLayout;
