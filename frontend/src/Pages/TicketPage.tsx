import { useEffect, useState } from "react";
import TicketList from "../components/TicketList";
import type { TicketModel } from "../models/TicketModel";

const Ticket = () => {
  const [tickets, setTickets] = useState<TicketModel[]>([]);

  const [isEditing, setIsEditing] = useState<boolean>(false)
  const [selectedId, setSelectedId] = useState<string>("")
  const [quantity, setQuantity] = useState<number>(0)

  useEffect(() => {
    getTickets();
  }, []);

  const getTickets = async () => {
    setTickets([
      {
        id: "1",
        eventName: "Trka 1",
        zoneName: "",
        date: "",
        price: 100,
        currency: "",
        quantity: 5,
      },
      {
        id: "2",
        eventName: "Trka 2",
        zoneName: "",
        date: "",
        price: 100,
        currency: "",
        quantity: 5,
      },
      {
        id: "3",
        eventName: "Trka 3",
        zoneName: "",
        date: "",
        price: 100,
        currency: "",
        quantity: 5,
      },
    ]);
  };

  const onDeleteClick = (id: string) => {
    
  };
  const onEditClick = (id: string) => {
    setIsEditing(true)
    setSelectedId(id)
  };

  const onEditConfirmClick = () => {
   console.log(quantity, selectedId)
  ///API  quantity, selectedId
   setQuantity(0)
   setIsEditing(false)
  };

  return (
    <TicketList
      tickets={tickets}
      onDeleteClick={onDeleteClick}
      onEditClick={onEditClick}
      onEditConfirmClick={onEditConfirmClick}
      quantity={quantity}
      onQuantityChange={(newValue) => setQuantity(newValue)}
      isEditing={isEditing}
      selectedId={selectedId}
    ></TicketList>
  );
};

export default Ticket;
