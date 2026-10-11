// src/pages/UserTicketsPage.tsx
import { useEffect, useState } from "react";
import { useAuth } from "../context/AuthContext";
import {
  getUserTickets,
  updateTicket,
  cancelTicket,
} from "../services/ticketService";
import type { TicketModel, ZoneOption } from "../models/TicketModel";

export default function UserTicketsPage() {
  const { user } = useAuth();

  const [tickets, setTickets] = useState<TicketModel[]>([]);
  const [loading, setLoading] = useState(true);
  const [updatingTicketId, setUpdatingTicketId] = useState<string | null>(null);
  const [cancellingTicketId, setCancellingTicketId] = useState<string | null>(
    null
  );
  const [selectedZones, setSelectedZones] = useState<Record<string, string>>(
    {}
  );

  // CENTRALNA FUNKCIJA ZA FETCH
  const fetchTickets = async () => {
    if (!user) return;

    setLoading(true);

    try {
      const data = await getUserTickets(user.id);

      const mappedTickets: TicketModel[] = data.map((t: any) => ({
        ticketId: t.ticketId,
        email: t.email || user.email,
        eventName: t.eventName,
        raceDay: t.raceDay,
        zoneName: t.zoneName,
        quantity: t.quantity,
        totalPrice: t.totalPrice,
        currencyCode: t.currencyCode,
        status: t.status,
        accessCode: t.accessCode,
        availableZones: t.availableZones || [],
        currentZoneDayId: t.currentZoneDayId || t.availableZones?.[0]?.id,
      }));

      setTickets(mappedTickets);
    } catch (err) {
      console.error("Failed to fetch tickets:", err);
    } finally {
      setLoading(false);
    }
  };

  // FETCH kada se stranica otvori
  useEffect(() => {
    fetchTickets();
  }, [user]);

  if (!user)
    return (
      <div className="p-6 text-center mt-24">
        Please log in to see your tickets
      </div>
    );

  if (loading)
    return <div className="p-6 text-center mt-24">Loading your tickets...</div>;

  const handleZoneChange = (ticketId: string, zoneId: string) => {
    setSelectedZones((prev) => ({ ...prev, [ticketId]: zoneId }));
  };

  // UPDATE TICKET
  const handleConfirmUpdate = async (ticket: TicketModel) => {
    const newZoneDayId =
      selectedZones[ticket.ticketId] ?? ticket.currentZoneDayId;
    const currentZoneDayId = ticket.currentZoneDayId;

    if (!newZoneDayId || !currentZoneDayId) {
      alert("Please select a valid zone first!");
      return;
    }

    if (newZoneDayId === currentZoneDayId) {
      alert("You selected the same zone.");
      return;
    }

    setUpdatingTicketId(ticket.ticketId);

    try {
      await updateTicket({
        ticketId: ticket.ticketId,
        accessCode: ticket.accessCode,
        email: ticket.email,
        removeZoneDayId: currentZoneDayId,
        addZoneDayId: newZoneDayId,
      });

      alert("Ticket updated successfully!");

      // REFRESH PODATAKA
      await fetchTickets();
    } catch (err) {
      console.error("Failed to update ticket:", err);
      alert("Failed to update ticket.");
    } finally {
      setUpdatingTicketId(null);
    }
  };

  // CANCEL TICKET
  const handleCancelTicket = async (ticket: TicketModel) => {
    if (!window.confirm("Are you sure you want to cancel this ticket?")) return;

    setCancellingTicketId(ticket.ticketId);

    try {
      await cancelTicket({
        ticketId: ticket.ticketId,
        accessCode: ticket.accessCode,
        email: ticket.email,
      });

      alert("Ticket cancelled successfully!");

      // REFRESH PODATAKA
      await fetchTickets();
    } catch (err) {
      console.error("Failed to cancel ticket:", err);
      alert("Failed to cancel ticket.");
    } finally {
      setCancellingTicketId(null);
    }
  };

  return (
    <div className="max-w-6xl mx-auto p-6 pt-28">
      <h1 className="text-3xl font-bold mb-6">My Tickets</h1>

      {tickets.length === 0 && (
        <p className="text-gray-400">You have no tickets yet.</p>
      )}

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
        {tickets.map((ticket) => (
          <div
            key={ticket.ticketId}
            className="bg-zinc-800 p-5 rounded-xl shadow-lg flex flex-col gap-4"
          >
            <div>
              <h2 className="text-xl font-semibold text-white">
                {ticket.eventName}
              </h2>

              <p className="text-gray-300">
                Date: {new Date(ticket.raceDay).toLocaleDateString()}
              </p>

              <p className="text-gray-300">Zone: {ticket.zoneName}</p>

              <p className="text-gray-300">Quantity: {ticket.quantity}</p>

              <p className="text-gray-300">
                Total: {ticket.totalPrice} {ticket.currencyCode}
              </p>

              <p
                className={`font-semibold text-white`}
              >
                Status: {ticket.status}
              </p>
              {ticket.availableZones?.length > 0 &&
                ticket.status !== "Cancelled" && (
                  <select
                    className="mt-2 w-full bg-zinc-700 text-white p-2 rounded"
                    value={
                      selectedZones[ticket.ticketId] ??
                      ticket.currentZoneDayId
                    }
                    onChange={(e) =>
                      handleZoneChange(ticket.ticketId, e.target.value)
                    }
                  >
                    {ticket.availableZones.map((z: ZoneOption) => (
                      <option key={z.id} value={z.id}>
                        {z.zoneName} | seats left: {z.seatsLeft} | +€
                        {z.priceAddon}
                      </option>
                    ))}
                  </select>
                )}
            </div>

            <div className="flex gap-2">
              <button
                disabled={
                  updatingTicketId === ticket.ticketId ||
                  ticket.status === "Cancelled"
                }
                className="flex-1 px-4 py-2 bg-yellow-600 hover:bg-yellow-700 text-white rounded"
                onClick={() => handleConfirmUpdate(ticket)}
              >
                {updatingTicketId === ticket.ticketId
                  ? "Updating..."
                  : "Update"}
              </button>

              <button
                disabled={
                  cancellingTicketId === ticket.ticketId ||
                  ticket.status === "Cancelled"
                }
                className="flex-1 px-4 py-2 bg-red-600 hover:bg-red-700 text-white rounded"
                onClick={() => handleCancelTicket(ticket)}
              >
                {cancellingTicketId === ticket.ticketId
                  ? "Cancelling..."
                  : "Cancel"}
              </button>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}