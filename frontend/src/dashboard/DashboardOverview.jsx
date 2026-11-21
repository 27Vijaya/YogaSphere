import "./DashboardOverview.css";

function DashboardOverview() {
  return (
    <div className="dashboard-overview">
      <h2>Welcome to YogaSphere!</h2>

      <p>
        Your personal space to manage your yoga journey.
      </p>

      {/* Dashboard summary cards */}
      <div className="dashboard-cards">
        <div className="dashboard-card">
          <h3>Yoga Programs</h3>
          <p>Explore available programs</p>
        </div>

        <div className="dashboard-card">
          <h3>Yoga Classes</h3>
          <p>Discover upcoming classes</p>
        </div>

        <div className="dashboard-card">
          <h3>My Bookings</h3>
          <p>Manage your class bookings</p>
        </div>
      </div>
    </div>
  );
}

export default DashboardOverview;