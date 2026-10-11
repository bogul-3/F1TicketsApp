import { useState, useEffect } from "react";
import CrudTable from "../../../components/admin/common/CrudTable";
import RaceDayForm from "./RaceDayForm";
import type { RaceDay } from "../../../models/RaceDay";
import { getRaceDays, deleteRaceDay } from "../../../services/admin/raceDayService";

type Props = {
  raceEventId: string;
  startDate: string; 
  endDate: string;   
  onBack: () => void;
  onSelectRaceDay?: (raceDayId: string) => void; // callback za ZoneDays
};

type View = "LIST" | "CREATE" | "EDIT";

export default function RaceDaysPage({ raceEventId, startDate, endDate, onBack, onSelectRaceDay }: Props) {
  const [raceDays, setRaceDays] = useState<RaceDay[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [view, setView] = useState<View>("LIST");
  const [selectedRaceDay, setSelectedRaceDay] = useState<RaceDay | null>(null);

  useEffect(() => {
    fetchRaceDays();
  }, []);

  const fetchRaceDays = async () => {
    setIsLoading(true);
    try {
      const res = await getRaceDays(raceEventId);
      setRaceDays(res);
    } catch (err) {
      console.error(err);
    } finally {
      setIsLoading(false);
    }
  };

  const handleDelete = async (rd: RaceDay) => {
    const confirmDelete = window.confirm("Delete this race day?");
    if (!confirmDelete) return;

    setIsLoading(true);
    try {
      await deleteRaceDay(rd.id);
      setRaceDays(prev => prev.filter(r => r.id !== rd.id));
    } catch (err) {
      console.error(err);
      alert("Failed to delete race day");
    } finally {
      setIsLoading(false);
    }
  };

  const handleSuccess = () => {
    setView("LIST");
    fetchRaceDays();
  };

  const handleSelect = (rd: RaceDay) => {
    if (onSelectRaceDay) onSelectRaceDay(rd.id);
  };

  if (view === "CREATE") {
    return (
      <RaceDayForm
        mode="create"
        raceEventId={raceEventId}
        startDate={startDate} // <- ISO string
        endDate={endDate}     // <- ISO string
        onSuccess={handleSuccess}
      />
    );
  }

  if (view === "EDIT" && selectedRaceDay) {
    return (
      <RaceDayForm
        mode="edit"
        initialData={selectedRaceDay}
        startDate={startDate} // <- ISO string
        endDate={endDate}     // <- ISO string
        onSuccess={handleSuccess}
      />
    );
  }

  // --- Render list view ---
  return (
    <CrudTable<RaceDay>
      title="Race Days"
      data={raceDays}
      isLoading={isLoading}
      columns={[
        { header: "Date", render: rd => new Date(rd.date).toLocaleDateString() },
        { header: "Base Price", render: rd => `$${rd.basePrice.toFixed(2)}` },
      ]}
      onCreate={() => setView("CREATE")}
      onEdit={(rd) => { setSelectedRaceDay(rd); setView("EDIT"); }}
      onDelete={handleDelete}
      onBack={onBack}
      onSelect={handleSelect} // dugme Manage Zone Days
    />
  );
}