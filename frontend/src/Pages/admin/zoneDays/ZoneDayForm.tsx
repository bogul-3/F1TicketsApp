import { useState, useEffect } from "react";
import type { ZoneDay } from "../../../models/ZoneDay";
import type { Zone } from "../../../models/Zone";
import { getZones } from "../../../services/admin/zoneService";
import { createZoneDay, updateZoneDay } from "../../../services/admin/zoneDayService";

type Props =
  | { mode: "create"; raceDayId: string; onSuccess: () => void }
  | { mode: "edit"; initialData: ZoneDay; onSuccess: () => void };

export default function ZoneDayForm(props: Props) {
  const { mode, onSuccess } = props;

  const raceDayId = mode === "create" ? props.raceDayId : props.initialData.raceDayId;
  const initialData = mode === "edit" ? props.initialData : undefined;

  const [zones, setZones] = useState<Zone[]>([]);
  const [selectedZone, setSelectedZone] = useState(initialData?.zoneId || "");
  const [capacity, setCapacity] = useState(initialData?.capacity || 0);
  const [priceAddon, setPriceAddon] = useState(initialData?.priceAddon || 0);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    getZones().then(setZones).catch(console.error);
  }, []);

  const handleSubmit = async () => {
    setError("");

    // 1. Validation
    if (mode === "create" && !selectedZone) {
      setError("Please select a zone");
      return;
    }

    if (capacity <= 0) {
      setError("Capacity must be greater than 0");
      return;
    }

    if (priceAddon < 0) {
      setError("Price addon cannot be negative");
      return;
    }

    // 2. Submit
    setIsLoading(true);
    try {
      if (mode === "create") {
        await createZoneDay(raceDayId, { zoneId: selectedZone, capacity, priceAddon });
        alert("ZoneDay created");
      } else if (mode === "edit" && initialData) {
        await updateZoneDay(initialData.id, { capacity, priceAddon });
        alert("ZoneDay updated");
      }
      onSuccess();
    } catch (err) {
      console.error(err);
      setError("Operation failed");
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="bg-white shadow-lg rounded-lg p-6 max-w-md mx-auto mt-6">
      <h2 className="text-2xl font-bold text-gray-800 mb-6 text-center">
        {mode === "create" ? "Create Zone Day" : "Edit Zone Day"}
      </h2>

      <div className="flex flex-col gap-4">
        {mode === "create" && (
          <select
            className="border border-gray-300 rounded-lg p-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
            value={selectedZone}
            onChange={(e) => setSelectedZone(e.target.value)}
          >
            <option value="">Select Zone</option>
            {zones.map(z => <option key={z.id} value={z.id}>{z.name}</option>)}
          </select>
        )}

        {/* Capacity */}
        <div className="flex flex-col">
          <label htmlFor="capacity" className="text-gray-700 font-medium mb-1">Capacity</label>
          <input
            id="capacity"
            type="number"
            className="border border-gray-300 rounded-lg p-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
            value={capacity}
            min={1}
            onChange={e => setCapacity(Number(e.target.value))}
            placeholder="Capacity"
          />
        </div>

        {/* Price Addon */}
        <div className="flex flex-col">
          <label htmlFor="priceAddon" className="text-gray-700 font-medium mb-1">Price Addon</label>
          <input
            id="priceAddon"
            type="number"
            className="border border-gray-300 rounded-lg p-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
            value={priceAddon}
            min={0}
            step={0.01}
            onChange={e => setPriceAddon(Number(e.target.value))}
            placeholder="Price Addon"
          />
        </div>

        {/* Error */}
        {error && <p className="text-red-600 font-medium mt-1">{error}</p>}
      </div>

      <div className="flex gap-4 mt-6">
        <button
          onClick={handleSubmit}
          disabled={isLoading}
          className={`flex-1 text-white py-2 rounded-lg font-semibold transition-colors duration-200
            ${mode === "create" ? "bg-green-600 hover:bg-green-700" : "bg-blue-600 hover:bg-blue-700"}
            ${isLoading ? "opacity-50 cursor-not-allowed" : ""}`}
        >
          {mode === "create" ? "Create" : "Update"}
        </button>

        <button
          onClick={onSuccess}
          className="flex-1 bg-gray-300 hover:bg-gray-400 text-gray-800 font-semibold py-2 rounded-lg transition-colors duration-200"
        >
          Cancel
        </button>
      </div>
    </div>
  );
}