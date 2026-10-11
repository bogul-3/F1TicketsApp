import type { RaceDay } from "../../../models/RaceDay"

type Props = {
  raceDays: RaceDay[]
  isLoading: boolean
  onCreate: () => void
  onEdit: (day: RaceDay) => void
  onDelete: (id: string) => void
  onBack: () => void
}

export default function RaceDaysTable({
  raceDays,
  isLoading,
  onCreate,
  onEdit,
  onDelete,
  onBack
}: Props) {

  if (isLoading) {
    return <p className="text-gray-500">Loading race days...</p>
  }

  if (raceDays.length === 0) {
    return (
      <div className="flex flex-col gap-4">

        <button
          onClick={onBack}
          className="bg-gray-500 text-white px-4 py-2 rounded w-fit hover:bg-gray-600"
        >
          Back to Events
        </button>

        <h2 className="text-2xl font-semibold">Race Days</h2>

        <p className="text-gray-500">No race days yet.</p>

        <button
          onClick={onCreate}
          className="bg-green-600 text-white px-4 py-2 rounded w-fit hover:bg-green-700"
        >
          Add Race Day
        </button>

      </div>
    )
  }

  const handleDelete = async (id: string) => {
    if (!window.confirm("Delete this race day?")) return
    await onDelete(id)
  }

  return (
    <div className="flex flex-col gap-6">

      <div className="flex justify-between items-center">

        <div className="flex items-center gap-3">

          <button
            onClick={onBack}
            className="bg-gray-500 text-white px-4 py-2 rounded hover:bg-gray-600"
          >
            Back
          </button>

          <h2 className="text-2xl font-semibold">
            Race Days
          </h2>

        </div>

        <button
          onClick={onCreate}
          className="bg-green-600 text-white px-4 py-2 rounded hover:bg-green-700"
        >
          Add Race Day
        </button>

      </div>

      <table className="w-full border border-gray-200 shadow-sm">

        <thead className="bg-gray-100">

          <tr>
            <th className="p-3 text-left">Date</th>
            <th className="p-3 text-left">Base Price</th>
            <th className="p-3 text-center">Actions</th>
          </tr>

        </thead>

        <tbody>

          {raceDays.map(day => (

            <tr
              key={day.id}
              className="border-t hover:bg-gray-50"
            >

              <td className="p-3">
                {new Date(day.date).toLocaleDateString()}
              </td>

              <td className="p-3">
                {day.basePrice}
              </td>

              <td className="p-3 flex justify-center gap-2">

                <button
                  onClick={() => onEdit(day)}
                  className="bg-blue-500 text-white px-3 py-1 rounded hover:bg-blue-600"
                >
                  Edit
                </button>

                <button
                  onClick={() => handleDelete(day.id)}
                  className="bg-red-500 text-white px-3 py-1 rounded hover:bg-red-600"
                >
                  Delete
                </button>

              </td>

            </tr>

          ))}

        </tbody>

      </table>

    </div>
  )
}