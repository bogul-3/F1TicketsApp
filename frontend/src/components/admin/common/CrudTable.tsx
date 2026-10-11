import React from "react"

type Column<T> = {
  header: string
  render: (item: T) => React.ReactNode
}

type Props<T> = {
  title: string
  data: T[]
  isLoading: boolean
  columns: Column<T>[]
  onCreate?: () => void
  onEdit?: (item: T) => void
  onDelete?: (item: T) => void
  onBack?: () => void
  onSelect?: (item: T) => void
}

export default function CrudTable<T>({
  title,
  data,
  isLoading,
  columns,
  onCreate,
  onEdit,
  onDelete,
  onBack,
  onSelect
}: Props<T>) {

  if (isLoading) return <p className="text-gray-500">Loading...</p>

  return (
    <div className="flex flex-col gap-6">

      <div className="flex justify-between items-center">

        <div className="flex gap-3 items-center">

          {onBack && (
            <button
              onClick={onBack}
              className="bg-gray-500 text-white px-4 py-2 rounded"
            >
              Back
            </button>
          )}

          <h2 className="text-2xl font-semibold">{title}</h2>

        </div>

        {onCreate && (
          <button
            onClick={onCreate}
            className="bg-green-600 text-white px-4 py-2 rounded"
          >
            Create
          </button>
        )}

      </div>

      {data.length === 0 ? (
        <p className="text-gray-500">No data.</p>
      ) : (

        <table className="w-full border border-gray-200 shadow-sm">

          <thead className="bg-gray-100">
            <tr>
              {columns.map((col, i) => (
                <th key={i} className="p-3 text-left">{col.header}</th>
              ))}
              {(onEdit || onDelete) && (
                <th className="p-3 text-center">Actions</th>
              )}
            </tr>
          </thead>

          <tbody>

            {data.map((item, i) => (

              <tr key={i} className="border-t hover:bg-gray-50">

                {columns.map((col, ci) => (
                  <td key={ci} className="p-3">
                    {col.render(item)}
                  </td>
                ))}

                {(onEdit || onDelete) && (
                  <td className="p-3 flex justify-center gap-2">

                    {onEdit && (
                      <button
                        onClick={() => onEdit(item)}
                        className="bg-blue-500 text-white px-3 py-1 rounded"
                      >
                        Edit
                      </button>
                    )}

                    {onDelete && (
                      <button
                        onClick={() => onDelete(item)}
                        className="bg-red-500 text-white px-3 py-1 rounded"
                      >
                        Delete
                      </button>
                    )}

                    {onSelect && (
                      <button
                        onClick={() => onSelect(item)}
                        className="bg-green-600 text-white px-4 py-2 rounded"
                      >
                        Zone Days
                      </button>
                    )}

                  </td>
                )}

              </tr>

            ))}

          </tbody>

        </table>

      )}

    </div>
  )
}