
import { useEffect, useState } from "react";
import request from "../services/api";
import "./DashboardClasses.css";

function DashboardClasses() {
  const [classes, setClasses] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  // Program ID 4 has existing class data.
  // We'll make this dynamic when we add program selection.
  const programId = 4;

  // Fetch classes belonging to the selected program
  useEffect(() => {
    async function loadClasses() {
      try {
        const data = await request(
          `/YogaClass/program/${programId}`
        );

        console.log("Yoga Classes API response:", data);

        setClasses(Array.isArray(data) ? data : []);
      } catch (err) {
        console.error("Failed to load yoga classes:", err);
        setError(err.message);
      } finally {
        setLoading(false);
      }
    }

    loadClasses();
  }, []);

  return (
    <div className="dashboard-classes">
      <h2>Yoga Classes</h2>
      <p>Discover classes for your wellness journey.</p>

      {loading && <p>Loading yoga classes...</p>}

      {error && <p className="error-message">{error}</p>}

      {!loading && !error && classes.length === 0 && (
        <p>No yoga classes are currently available.</p>
      )}

      {!loading && !error && classes.length > 0 && (
        <div className="class-list">
          {classes.map((yogaClass) => (
            <article className="class-card" key={yogaClass.id}>
              <div className="class-card-header">
                <h3>{yogaClass.name}</h3>

                <span
                  className={
                    yogaClass.isActive
                      ? "active-status"
                      : "inactive-status"
                  }
                >
                  {yogaClass.isActive ? "Active" : "Inactive"}
                </span>
              </div>

              <p className="class-description">
                {yogaClass.description}
              </p>

              <div className="class-info">
                <p>
                  <strong>Duration:</strong>{" "}
                  {yogaClass.durationMinutes} minutes
                </p>

                <p>
                  <strong>Capacity:</strong>{" "}
                  {yogaClass.capacity} participants
                </p>
              </div>
            </article>
          ))}
        </div>
      )}
    </div>
  );
}

export default DashboardClasses;
