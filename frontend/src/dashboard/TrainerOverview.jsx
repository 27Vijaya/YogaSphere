
import { useEffect, useState } from "react";
import request from "../services/api";
import "./TrainerOverview.css";

function TrainerOverview() {
  const [programs, setPrograms] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  // Load programs belonging to the logged-in trainer.
  useEffect(() => {
    async function loadPrograms() {
      try {
        const data = await request("/YogaProgram/my-programs");
        setPrograms(data);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    }

    loadPrograms();
  }, []);

  return (
    <div className="trainer-overview">
      <div className="trainer-page-header">
        <div>
          <h1>Trainer Overview</h1>
          <p>Welcome to your YogaSphere trainer workspace.</p>
        </div>
      </div>

      {loading ? (
        <p>Loading your dashboard...</p>
      ) : error ? (
        <p className="trainer-error">{error}</p>
      ) : (
        <>
          <div className="trainer-stats">
            <div className="trainer-stat-card">
              <span>My Programs</span>
              <strong>{programs.length}</strong>
            </div>

            <div className="trainer-stat-card">
              <span>Active Programs</span>
              <strong>
                {programs.filter((program) => program.isActive).length}
              </strong>
            </div>
          </div>

          <section className="trainer-section">
            <h2>My Programs</h2>

            {programs.length === 0 ? (
              <p>No programs assigned yet.</p>
            ) : (
              <div className="trainer-program-grid">
                {programs.map((program) => (
                  <article className="trainer-program-card" key={program.id}>
                    <h3>{program.name}</h3>
                    <p>{program.description || "No description available."}</p>

                    <div className="trainer-program-details">
                      <span>{program.durationWeeks} weeks</span>
                      <span>₹{program.price}</span>
                    </div>

                    <span
                      className={
                        program.isActive
                          ? "trainer-status active"
                          : "trainer-status inactive"
                      }
                    >
                      {program.isActive ? "Active" : "Inactive"}
                    </span>
                  </article>
                ))}
              </div>
            )}
          </section>
        </>
      )}
    </div>
  );
}

export default TrainerOverview;
