import { X } from "lucide-react";
import { useCart } from "../hooks/useCart";
import { Book } from "../types/types";
import { Button } from "./ui/button";

interface BookCardProps {
    book: Book;
}

export const BookCard = ({ book }: BookCardProps) => {
    const { removeBook } = useCart();

    return (
        <div className="p-4 border border-slate-300 rounded-lg flex gap-8 items-center justify-between">
            <div className="flex flex-col gap-1">
                <p className="font-bold line-clamp-2">{book.title}</p>
                <p>by {book.author}</p>
                <p className="text-sm text-slate-600">ID: {book.id}</p>
            </div>
            <Button variant="outline" size="icon" onClick={() => removeBook(book.id)}>
                <X />
            </Button>
        </div>
    )
}