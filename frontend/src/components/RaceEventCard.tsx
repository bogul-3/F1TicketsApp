import type { RaceEventHome } from "../models/RaceEvent";
import { Link } from "react-router-dom";

interface Props {
  event: RaceEventHome;
}

const RaceEventCard = ({ event }: Props) => {
  return (
    <div className="bg-zinc-900 text-white rounded-xl shadow-lg p-6 border border-zinc-700 hover:border-red-500 transition">

      {/* Event title */}
      <h2 className="text-2xl font-bold mb-2">{event.name}</h2>

      {/* City */}
      <p className="text-gray-400 mb-1">City: {event.city}</p>

      {/* Dates */}
      <p className="text-gray-400 mb-4">
        {event.startDate} - {event.endDate}
      </p>

      <h3 className="text-lg font-semibold mb-3 text-red-400">Race Days</h3>

      <div className="space-y-4">
        {event.raceDays.map((day) => (
          <div
            key={day.id}
            className="bg-zinc-800 p-4 rounded-lg border border-zinc-700"
          >
            <p className="font-semibold">Date: {day.date}</p>
            <p className="text-gray-300 mb-2">
              Base price: ${day.basePrice}
            </p>

            <h4 className="text-sm text-red-400 mb-2">Zones</h4>

            <div className="space-y-2">
              {day.zoneDays.map((zone) => (
                <div
                  key={zone.zoneDayId}
                  className="flex justify-between text-sm bg-zinc-700 px-3 py-2 rounded"
                >
                  <span>Seats left: {zone.seatsLeft}</span>
                  <span>+${zone.priceAddon}</span>
                </div>
              ))}
            </div>
          </div>
        ))}
      </div>

      {/* Booking button */}
      <Link
        to={`/booking/${event.id}`}
        className="block mt-6 text-center bg-red-600 hover:bg-red-700 transition px-4 py-2 rounded-lg font-semibold"
      >
        Book Tickets
      </Link>
    </div>
  );
};

export default RaceEventCard;