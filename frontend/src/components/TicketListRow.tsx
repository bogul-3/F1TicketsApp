import { type FC } from "react";
import type { TicketModel } from "../models/TicketModel.js";

interface Props {
  ticket: TicketModel;
  isEditing: boolean;
  quantity: number;
  onQuantityChange: (e) => void;
  onDeleteClick?: () => void;
  onEditClick?: () => void;
  onEditConfirmClick?: () => void;
}

const TicketListRow: FC<Props> = ({
  ticket,
  onDeleteClick,
  onEditClick,
  onEditConfirmClick,
  quantity,
  onQuantityChange,
  isEditing,
}) => {
  return (
    <tr
      key={ticket.id}
      className={`
        border-b transition-colors
      `}
    >
      <td className="px-6 py-3">{ticket.eventName}</td>
      <td className="px-6 py-3">{ticket.zoneName}</td>
      <td className="px-6 py-3">{ticket.date}</td>
      <td className="px-6 py-3">
        {ticket.price} {ticket.currency}
      </td>
      <td className="px-6 py-3">{ticket.quantity}</td>
      <td className="px-6 py-3 cursor-pointer" onClick={onDeleteClick}>
        Delete
      </td>
      {isEditing ? (
        <div>
          <input
            type="number"
            value={quantity}
            onChange={(e) => onQuantityChange(e.target.value)}
          ></input>
          <button onClick={() => onEditConfirmClick?.()}>Confirm</button>
        </div>
      ) : (
        <td className="px-6 py-3 cursor-pointer" onClick={onEditClick}>
          Edit
        </td>
      )}
    </tr>
  );
};

export default TicketListRow;
