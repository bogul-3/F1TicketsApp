import type { Zone } from "../../../models/Zone"

type Props = {
  zones: Zone[]
  isLoading: boolean
  onCreate: () => void
  onEdit: (zone: Zone) => void
  onDelete: (id: string) => void
}

export default function ZonesTable({ zones, isLoading, onCreate, onEdit, onDelete }: Props) {
  if (isLoading) return <p>Loading zones...</p>
  if (zones.length === 0)
    return (
      <div className="flex flex-col gap-4">
        <h2 className="text-2xl font-semibold">Zones</h2>
        <p>No zones yet.</p>
        <button onClick={onCreate} className="bg-green-600 text-white px-4 py-2 rounded">Add Zone</button>
      </div>
    )

  return (
    <div className="flex flex-col gap-6">
      <div className="flex justify-between items-center">
        <h2 className="text-2xl font-semibold">Zones</h2>
        <button onClick={onCreate} className="bg-green-600 text-white px-4 py-2 rounded hover:bg-green-700">Add Zone</button>
      </div>

      <table className="w-full border border-gray-200 shadow-sm">
        <thead className="bg-gray-100">
          <tr>
            <th className="p-3 text-left">Name</th>
            <th className="p-3 text-center">Actions</th>
          </tr>
        </thead>
        <tbody>
          {zones.map(z => (
            <tr key={z.id} className="border-t hover:bg-gray-50">
              <td className="p-3">{z.name}</td>
              <td className="p-3 flex justify-center gap-2">
                <button onClick={() => onEdit(z)} className="bg-blue-500 text-white px-3 py-1 rounded hover:bg-blue-600">Edit</button>
                <button onClick={() => onDelete(z.id)} className="bg-red-500 text-white px-3 py-1 rounded hover:bg-red-600">Delete</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}