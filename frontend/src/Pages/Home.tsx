import { useEffect, useState } from "react";
import { getRaceEvents } from "../services/raceEventServiceHomeService";
import type { RaceEventHome } from "../models/RaceEvent";
import RaceEventCard from "../components/RaceEventCard";

const HomePage = () => {
  const [events, setEvents] = useState<RaceEventHome[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchEvents = async () => {
      try {
        const data = await getRaceEvents();
        setEvents(data);
      } catch (error) {
        console.error(error);
      } finally {
        setLoading(false);
      }
    };

    fetchEvents();
  }, []);

  if (loading) {
    return <div>Loading...</div>;
  }

  return (
    <div>
      <h1 className="text-3xl font-bold mb-6">Race Events</h1>

      <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-6">
        {events.map((event) => (
          <RaceEventCard key={event.id} event={event} />
        ))}
      </div>
    </div>
  );
};

export default HomePage;
