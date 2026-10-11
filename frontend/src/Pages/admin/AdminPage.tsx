import { useState } from "react";
import RaceEventsPage from "./raceEvents/RaceEventsPage";
import RaceDaysPage from "./raceDays/RaceDaysPage";
import ZoneDaysPage from "./zoneDays/ZoneDaysPage"; // tvoja nova stranica za ZoneDays
import ZonesPage from "./zones/ZonesPage";
import type { RaceEvent } from "../../models/RaceEvent";
import ReportsPage from "./reports/ReportsPage";

type AdminView = "EVENTS" | "DAYS" | "ZONES" | "ZONE_DAYS" | "REPORTS";

export default function AdminPage() {
  const [view, setView] = useState<AdminView>("EVENTS");
  const [selectedEvent, setSelectedEvent] = useState<RaceEvent | null>(null);
  const [selectedRaceDayId, setSelectedRaceDayId] = useState<string | null>(null);

  return (
    <div className="pt-60 p-10 flex flex-col gap-4">
      {/* NAVIGATION BUTTONS */}
      <div className="flex gap-2">
        <button
          className="bg-blue-600 text-white px-4 py-2 rounded"
          onClick={() => setView("EVENTS")}
        >
          Manage Race Events
        </button>

        <button
          className="bg-purple-600 text-white px-4 py-2 rounded"
          onClick={() => setView("ZONES")}
        >
          Manage Zones
        </button>

        <button
          className="bg-orange-600 text-white px-4 py-2 rounded"
          onClick={() => setView("REPORTS")}
        >
          Reports
        </button>
      </div>

      {/* MAIN VIEWS */}
      {view === "EVENTS" && (
        <RaceEventsPage
          onManageDays={(event) => {
            setSelectedEvent(event);
            setView("DAYS");
          }}
        />
      )}

      {view === "DAYS" && selectedEvent && (
        <RaceDaysPage
          raceEventId={selectedEvent.id}
          startDate={selectedEvent.startDate}
          endDate={selectedEvent.endDate}
          onBack={() => setView("EVENTS")}
          onSelectRaceDay={(raceDayId) => {
            setSelectedRaceDayId(raceDayId);
            setView("ZONE_DAYS");
          }}
        />
      )}

      {view === "ZONE_DAYS" && selectedRaceDayId && (
        <ZoneDaysPage
          raceDayId={selectedRaceDayId}
          onBack={() => setView("DAYS")}
        />
      )}

      {view === "ZONES" && (
        <ZonesPage
          onBack={() => setView("EVENTS")}
        />
      )}

      {view === "REPORTS" && (
        <ReportsPage />
      )}
    </div>
  );
}