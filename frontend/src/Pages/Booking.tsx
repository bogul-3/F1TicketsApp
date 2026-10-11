import { useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import { getRaceEvent } from "../services/raceEventServiceHomeService";
import { useAuth } from "../context/AuthContext";
import { purchaseTickets } from "../services/ticketService";

const BookingPage = () => {
  const { id } = useParams();
  const { user } = useAuth();

  const [event, setEvent] = useState<any>(null);
  const [loading, setLoading] = useState(true);

  const [selectedZone, setSelectedZone] = useState<{ counterId: string; zoneDayId: string } | null>(null);
  const [quantity, setQuantity] = useState(1);
  const [currency, setCurrency] = useState("EUR");
  const [promoCode, setPromoCode] = useState<string | null>(null);
  const [purchaseResult, setPurchaseResult] = useState<any>(null);
  const [buyLoading, setBuyLoading] = useState(false);

  useEffect(() => {
    const fetchEvent = async () => {
      try {
        const data = await getRaceEvent(id!);
        setEvent(data);
        console.log(data);
      } catch (err) {
        console.error(err);
      } finally {
        setLoading(false);
      }
    };
    fetchEvent();
  }, [id]);

  if (loading) return <div className="p-6 text-center">Loading...</div>;
  if (!event) return <div className="p-6 text-center">Event not found</div>;

  // funkcija za max seats
  const getMaxSeats = () => {
    if (!selectedZone) return 1;
    for (const day of event.raceDays) {
      const zoneDay = day.zoneDays.find((z: any) => z.zoneDayId === selectedZone.zoneDayId);
      if (zoneDay) return zoneDay.seatsLeft;
    }
    return 1;
  };

  const handlePurchase = async () => {
    if (!selectedZone) return alert("Select a zone!");
    if (!user) return alert("You must be logged in!");

    setBuyLoading(true);
    try {
      const result = await purchaseTickets({
        zoneDayId: selectedZone.zoneDayId,
        userId: user.id,
        quantity,
        promoCode: promoCode,
        currencyCode: currency,
      });
      setPurchaseResult(result);
    } catch (err) {
      alert("Purchase failed!");
      console.error(err);
    } finally {
      setBuyLoading(false);
    }
  };

  return (
    <div className="max-w-5xl mx-auto p-6 space-y-6">
      {/* Event header */}
      <div className="bg-zinc-900 text-white p-6 rounded shadow-md">
        <h1 className="text-3xl font-bold mb-2">{event.name}</h1>
        <p className="text-gray-400 mb-1">{event.city}</p>
        <p className="text-gray-400 mb-4">
          {new Date(event.startDate).toLocaleDateString('sr-RS')} - {new Date(event.endDate).toLocaleDateString('sr-RS')}
        </p>
      </div>
      {/* RaceDays + Zones */}
      {event.raceDays
        .filter((day: any) => day.zoneDays && day.zoneDays.length > 0) // samo dani sa zone
        .map((day: any) => (
          <div key={day.id} className="bg-zinc-800 p-4 rounded shadow-md space-y-2">
            <h2 className="text-xl font-semibold mb-2 text-white">{new Date(day.date).toLocaleDateString('sr-RS')}</h2>
            {day.zoneDays.map((zoneDay: any, index: number) => {
              const counterId = `${day.id}-${index}`;
              const isSelected = selectedZone?.counterId === counterId;

              return (
                <label
                  key={counterId}
                  className={`flex items-center justify-between p-2 rounded cursor-pointer transition-colors
                    ${isSelected ? "bg-red-800" : "hover:bg-zinc-700"}`}
                >
                  <div className="flex items-center gap-2">
                    <input
                      type="radio"
                      name={`zone-${day.id}`} // grupi po danu
                      value={counterId}
                      checked={isSelected}
                      onChange={() =>
                        setSelectedZone({
                          counterId,
                          zoneDayId: zoneDay.zoneDayId, // PRAVI zoneDayId
                        })
                      }
                      className="accent-red-500 w-5 h-5"
                    />
                    <span className="text-white">
                      {zoneDay.name ?? `${zoneDay.zone.name}`} (+{zoneDay.priceAddon}€)
                    </span>
                  </div>
                  <span className="text-gray-300">Seats left: {zoneDay.seatsLeft}</span>
                </label>
              );
            })}
          </div>
        ))}

      {/* Quantity selector */}
      <div className="flex items-center gap-4">
        <button
          onClick={() => setQuantity(Math.max(1, quantity - 1))}
          className="px-4 py-2 bg-gray-700 rounded text-white hover:bg-gray-600 disabled:opacity-50 transition-colors"
          disabled={quantity <= 1}
        >
          -
        </button>
        <span className="text-xl font-semibold text-black">{quantity}</span>
        <button
          onClick={() => setQuantity(Math.min(getMaxSeats(), quantity + 1))}
          className="px-4 py-2 bg-gray-700 rounded text-white hover:bg-gray-600 transition-colors"
        >
          +
        </button>
      </div>

      {/* Promo code */}
      <input
        type="text"
        placeholder="Promo code"
        onChange={(e) => setPromoCode(e.target.value || null)}
        className="w-full p-2 rounded bg-zinc-700 text-white placeholder-gray-400"
      />

      {/* Currency */}
      <select
        value={currency}
        onChange={(e) => setCurrency(e.target.value)}
        className="w-full p-2 rounded bg-zinc-700 text-white"
      >
        <option value="EUR">EUR</option>
        <option value="USD">USD</option>
        <option value="RSD">RSD</option>
      </select>

      {/* Buy button */}
      <button
        onClick={handlePurchase}
        disabled={!selectedZone || !user || buyLoading}
        className="w-full py-3 bg-red-600 hover:bg-red-700 text-white font-bold rounded disabled:opacity-50 transition-colors"
      >
        {buyLoading ? "Processing..." : "Buy Tickets"}
      </button>

      {/* Purchase result */}
      {purchaseResult && (
        <div className="p-4 bg-green-600 text-white rounded shadow-md mt-4">
          <p className="font-semibold">Purchase successful!</p>
          <p>
            Total paid: {purchaseResult.totalPrice} {purchaseResult.currencyCode}
          </p>
          {purchaseResult.promoCode && <p>Your new promo code: {purchaseResult.promoCode}</p>}
        </div>
      )}
    </div>
  );
};

export default BookingPage;