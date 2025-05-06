import { Plus } from "lucide-react";
import { useParams } from "react-router-dom";
import { Button } from "../components/ui/button";
import { useBook } from "../hooks/useBook";
import { useCart } from "../hooks/useCart";

export const BookDetailsPage = () => {
    const { id } = useParams();
    const { data, isLoading, isError } = useBook(Number(id));
    const { addBook } = useCart();

    if (isLoading) return <p>Loading...</p>
    if (isError) return <p>Error getting books.</p>

    const book = data!;
    return (
        <div>
            <p className="font-bold">{book.title}</p>
            <p>by {book.author}</p>
            <p>Category: {book.category}</p>
            <p>{book.description}</p>
            <p>Book ID: {book.id}</p>
            <Button variant="outline" size="icon" onClick={() => addBook(book)}>
                <Plus />
            </Button>
        </div>
    )
}