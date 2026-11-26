
import { NavLink, Outlet } from "react-router-dom";
import {
  LayoutDashboard,
  BookOpen,
  CalendarDays,
  Leaf,
} from "lucide-react";

import "./TrainerDashboard.css";

function TrainerDashboard() {
  return (
    <div className="trainer-dashboard">
      <aside className="trainer-sidebar">
        <div className="trainer-brand">
          <div className="trainer-brand-icon">
            <Leaf size={25} />
          </div>

          <div>
            <h2>YogaSphere</h2>
            <span>Trainer Workspace</span>
          </div>
        </div>

        <div className="trainer-menu-label">MAIN MENU</div>

        <nav className="trainer-nav">
          <NavLink to="/trainer" end>
            <LayoutDashboard size={19} />
            <span>Overview</span>
          </NavLink>

          <NavLink to="/trainer/classes">
            <BookOpen size={19} />
            <span>My Classes</span>
          </NavLink>

          <NavLink to="/trainer/schedules">
            <CalendarDays size={19} />
            <span>My Schedules</span>
          </NavLink>
        </nav>

        <div className="trainer-sidebar-footer">
          <span>YogaSphere</span>
          <small>Trainer Portal</small>
        </div>
      </aside>

      <main className="trainer-content">
        <Outlet />
      </main>
    </div>
  );
}

export default TrainerDashboard;
