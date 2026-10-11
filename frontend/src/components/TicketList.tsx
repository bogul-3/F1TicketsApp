import { type FC } from "react";
import TicketListRow from "./TicketListRow.js";
import type { TicketModel } from "../models/TicketModel.js";

interface Props {
  tickets: TicketModel[];
  onDeleteClick: (id: string) => void;
  onEditClick: (id: string) => void;
  onEditConfirmClick: () => void;
  isEditing: boolean;
  selectedId: string;
  quantity: number
  onQuantityChange: (number) => void
}

const TicketList: FC<Props> = ({
  tickets,
  onDeleteClick,
  onEditClick,
  onEditConfirmClick,
  isEditing,
  selectedId,
  quantity,
  onQuantityChange
}) => {
  return (
    <div className="overflow-x-auto shadow-md sm:rounded-lg m-4">
      <table className="w-full text-sm text-left text-gray-500">
        <thead className="text-xs text-gray-700 uppercase bg-gray-100">
          <tr>
            <th className="px-6 py-3">Event Name</th>
            <th className="px-6 py-3">Zone Name</th>
            <th className="px-6 py-3">Date</th>
            <th className="px-6 py-3">Price</th>
            <th className="px-6 py-3">Quantity</th>
            <th className="px-6 py-3"></th>
            <th className="px-6 py-3"></th>
          </tr>
        </thead>
        <tbody>
          {tickets.map((ticket) => (
            <TicketListRow
              ticket={ticket}
              key={ticket.id}
              onDeleteClick={() => onDeleteClick(ticket.id)}
              onEditClick={() => onEditClick(ticket.id)}
              onEditConfirmClick={() => onEditConfirmClick()}
              isEditing={isEditing && selectedId === ticket.id}
              quantity={quantity}
              onQuantityChange={onQuantityChange}
            ></TicketListRow>
          ))}
        </tbody>
      </table>
    </div>
  );
};

export default TicketList;
