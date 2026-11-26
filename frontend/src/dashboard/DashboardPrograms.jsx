
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import request from "../services/api";
import "./DashboardPrograms.css";

function DashboardPrograms() {
  const [programs, setPrograms] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const navigate = useNavigate();

  // Fetch yoga programs from the backend
  useEffect(() => {
    async function loadPrograms() {
      try {
        const data = await request("/yogaprogram");

        console.log("Yoga Programs API response:", data);

        // Store the returned program records
        setPrograms(Array.isArray(data) ? data : []);
      } catch (err) {
        console.error("Failed to load yoga programs:", err);
        setError(err.message);
      } finally {
        setLoading(false);
      }
    }

    loadPrograms();
  }, []);

  return (
    <div className="dashboard-programs">
      <h2>Yoga Programs</h2>
      <p>Explore programs for your wellness journey.</p>

      {loading && <p>Loading yoga programs...</p>}

      {error && <p className="error-message">{error}</p>}

      {!loading && !error && programs.length === 0 && (
        <p>No yoga programs are currently available.</p>
      )}

      {!loading && !error && programs.length > 0 && (
        <div className="program-list">
          {programs.map((program) => (
            <article className="program-card" key={program.id}>
              <div className="program-card-header">
                <h3>{program.name}</h3>

                <span className={program.isActive ? "active-status" : "inactive-status"}>
                  {program.isActive ? "Active" : "Inactive"}
                </span>
              </div>

              <p className="program-description">
                {program.description}
              </p>

              <div className="program-info">
                <p>
                  <strong>Duration:</strong> {program.durationWeeks} weeks
                </p>

                <p>
                  <strong>Price:</strong> ₹{Number(program.price).toLocaleString("en-IN")}
                </p>
              </div>

              <button type="button" onClick={()=>navigate(`/dashboard/classes/${program.id}`)}>
                View Classes
              </button>
            </article>
          ))}
        </div>
      )}
    </div>
  );
}

export default DashboardPrograms;
