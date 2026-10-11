import type { ZoneDay } from "../../../models/ZoneDay";

type Props = {
  zoneDays: ZoneDay[];
  isLoading: boolean;
  onCreate: () => void;
  onEdit: (day: ZoneDay) => void;
  onDelete: (id: string) => void;
  onBack: () => void;
};

export default function ZoneDaysTable({ zoneDays, isLoading, onCreate, onEdit, onDelete, onBack }: Props) {
  if (isLoading) return <p>Loading...</p>;
  if (zoneDays.length === 0)
    return (
      <div className="flex flex-col gap-4">
        <h2 className="text-2xl font-semibold">Zone Days</h2>
        <p>No zone days yet.</p>
        <button onClick={onCreate} className="bg-green-600 text-white px-4 py-2 rounded">Add Zone Day</button>
        <button onClick={onBack} className="bg-gray-500 text-white px-4 py-2 rounded">Back</button>
      </div>
    );

  return (
    <div className="flex flex-col gap-6">
      <div className="flex justify-between items-center">
        <h2 className="text-2xl font-semibold">Zone Days</h2>
        <div className="flex gap-2">
          <button onClick={onCreate} className="bg-green-600 text-white px-4 py-2 rounded">Add Zone Day</button>
          <button onClick={onBack} className="bg-gray-500 text-white px-4 py-2 rounded">Back</button>
        </div>
      </div>

      <table className="w-full border border-gray-200 shadow-sm">
        <thead className="bg-gray-100">
          <tr>
            <th className="p-3 text-left">Zone</th>
            <th className="p-3 text-left">Seats Left</th>
            <th className="p-3 text-left">Capacity</th>
            <th className="p-3 text-left">Price Addon</th>
            <th className="p-3 text-center">Actions</th>
          </tr>
        </thead>
        <tbody>
          {zoneDays.map(day => (
            <tr key={day.id} className="border-t hover:bg-gray-50">
              <td className="p-3">{day.zoneName}</td>
              <td className="p-3">{day.seatsLeft}</td>
              <td className="p-3">{day.capacity}</td>
              <td className="p-3">{day.priceAddon.toFixed(2)}</td>
              <td className="p-3 flex justify-center gap-2">
                <button onClick={() => onEdit(day)} className="bg-blue-500 text-white px-3 py-1 rounded hover:bg-blue-600">Edit</button>
                <button onClick={() => onDelete(day.id)} className="bg-red-500 text-white px-3 py-1 rounded hover:bg-red-600">Delete</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}