import type { RaceEvent } from "../../../models/RaceEvent"

type Props = {
    events: RaceEvent[]
    isLoading: boolean
    onCreate: () => void
    onEdit: (event: RaceEvent) => void
    onDelete: (id: string) => void
    onManageDays: (event: RaceEvent) => void
}

export default function RaceEventsTable({
    events,
    isLoading,
    onCreate,
    onEdit,
    onDelete,
    onManageDays
}: Props) {
    const handleDelete = async (id: string) => {
        try {
            await onDelete(id)
        } catch (err) {
            console.error(err)
            alert("Failed to delete event")
        }
    }

    if (isLoading) {
        return <p className="text-gray-500">Loading race events...</p>
    }

    if (events.length === 0) {
        return (
            <div className="flex flex-col gap-4">

                <h2 className="text-2xl font-semibold">Race Events</h2>

                <p className="text-gray-500">No race events yet.</p>

                <button
                    onClick={onCreate}
                    className="bg-green-600 text-white px-4 py-2 rounded w-fit"
                >
                    Create Race Event
                </button>

            </div>
        )
    }

    return (
        <div className="flex flex-col gap-6">

            <div className="flex justify-between items-center">

                <h2 className="text-2xl font-semibold">Race Events</h2>

                <button
                    onClick={onCreate}
                    className="bg-green-600 text-white px-4 py-2 rounded hover:bg-green-700"
                >
                    Create Event
                </button>

            </div>

            <table className="w-full border border-gray-200 shadow-sm">

                <thead className="bg-gray-100">

                    <tr>
                        <th className="p-3 text-left">Name</th>
                        <th className="p-3 text-left">Start</th>
                        <th className="p-3 text-left">End</th>
                        <th className="p-3 text-left">Discount Until</th>
                        <th className="p-3 text-center">Actions</th>
                    </tr>

                </thead>

                <tbody>

                    {events.map(event => (

                        <tr
                            key={event.id}
                            className="border-t hover:bg-gray-50"
                        >

                            <td className="p-3 font-medium">
                                {event.name}
                            </td>

                            <td className="p-3">
                                {new Date(event.startDate).toLocaleString()}
                            </td>

                            <td className="p-3">
                                {new Date(event.endDate).toLocaleString()}
                            </td>

                            <td className="p-3">
                                {new Date(event.discountValidUntil).toLocaleDateString()}
                            </td>

                            <td className="p-3 flex justify-center gap-2">

                                <button
                                    onClick={() => onEdit(event)}
                                    className="bg-blue-500 text-white px-3 py-1 rounded hover:bg-blue-600"
                                >
                                    Edit
                                </button>

                                <button
                                    onClick={() => handleDelete(event.id)}
                                    className="bg-red-500 text-white px-3 py-1 rounded hover:bg-red-600"
                                >
                                    Delete
                                </button>

                                <button
                                    onClick={() => onManageDays(event)}
                                    className="bg-purple-500 text-white px-3 py-1 rounded hover:bg-purple-600"
                                >
                                    Race Days
                                </button>

                            </td>

                        </tr>

                    ))}

                </tbody>

            </table>

        </div>
    )
}