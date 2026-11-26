
import { useEffect, useState } from "react";
import request from "../services/api";
import "./TrainerClasses.css";

function TrainerClasses() {
  const [programs, setPrograms] = useState([]);
  const [selectedProgramId, setSelectedProgramId] = useState("");
  const [classes, setClasses] = useState([]);
  const [loadingPrograms, setLoadingPrograms] = useState(true);
  const [loadingClasses, setLoadingClasses] = useState(false);
  const [error, setError] = useState("");

  // Load programs belonging to the logged-in trainer.
  useEffect(() => {
    async function loadPrograms() {
      try {
        const data = await request("/YogaProgram/my-programs");

        const trainerPrograms = Array.isArray(data) ? data : [];
        setPrograms(trainerPrograms);

        if (trainerPrograms.length > 0) {
          setSelectedProgramId(String(trainerPrograms[0].id));
        }
      } catch (err) {
        setError(err.message);
      } finally {
        setLoadingPrograms(false);
      }
    }

    loadPrograms();
  }, []);

  // Load classes whenever the selected program changes.
  useEffect(() => {
    if (!selectedProgramId) {
      return;
    }

    let isActive = true;

    async function loadClasses() {
      setLoadingClasses(true);
      setError("");

      try {
        const data = await request(
          `/YogaClass/program/${selectedProgramId}`
        );

        if(isActive){
        setClasses(Array.isArray(data) ? data : []);
        }
      } catch (err) {
        if(isActive){
        setError(err.message);
        setClasses([]);
        }
      } finally {
        if(isActive){
        setLoadingClasses(false);
        }
      }
    }

    loadClasses();
     // Prevent an outdated request from updating state after the program changes.
  return () => {
    isActive = false;
  };
  }, [selectedProgramId]);

  return (
    <section className="trainer-classes">
      <h2>My Classes</h2>
      <p>Manage classes under your yoga programs.</p>

      {loadingPrograms && <p>Loading your programs...</p>}

      {!loadingPrograms && programs.length === 0 && (
        <p>No yoga programs found. Create a program first.</p>
      )}

      {!loadingPrograms && programs.length > 0 && (
        <>
          <div className="trainer-class-filter">
            <label htmlFor="programSelect">Select Program</label>

            <select
              id="programSelect"
              value={selectedProgramId}
              onChange={(e) => setSelectedProgramId(e.target.value)}
            >
              {programs.map((program) => (
                <option key={program.id} value={program.id}>
                  {program.name}
                </option>
              ))}
            </select>
          </div>

          {loadingClasses && <p>Loading classes...</p>}

          {!loadingClasses && !error && classes.length === 0 && (
            <p>No classes found for this program.</p>
          )}

          {!loadingClasses && classes.length > 0 && (
            <div className="trainer-class-list">
              {classes.map((yogaClass) => (
                <article className="trainer-class-card" key={yogaClass.id}>
                  <div className="trainer-class-header">
                    <h3>{yogaClass.name}</h3>
                    <span>
                      {yogaClass.isActive ? "Active" : "Inactive"}
                    </span>
                  </div>

                  <p>{yogaClass.description || "No description available."}</p>

                  <div className="trainer-class-info">
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
        </>
      )}

      {error && <p className="error-message">{error}</p>}
    </section>
  );
}

export default TrainerClasses;
