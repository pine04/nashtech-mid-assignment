import { ArrowLeft, Plus } from "lucide-react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { Button } from "../components/ui/button";
import { useBook } from "../hooks/useBook";
import { useCart } from "../hooks/useCart";
import { LoadingSpinner } from "../components/LoadingSpinner";
import { ErrorScreen } from "../components/ErrorScreen";
import { Badge } from "../components/ui/badge";

export const BookDetailsPage = () => {
    const { id } = useParams();
    const navigate = useNavigate();
    const { data, isLoading, isError } = useBook(Number(id));
    const { addBook } = useCart();

    if (isLoading) return <LoadingSpinner />;
    if (isError) return <ErrorScreen message="Error getting book." />;

    const book = data!;
    return (
        <div className="flex flex-col gap-4">
            <Button onClick={() => navigate(-1)} variant="outline" size="icon">
                <ArrowLeft />
            </Button>

            <h1 className="text-2xl font-bold">{book.title}</h1>

            <div className="flex flex-wrap gap-x-2 gap-y-1">
                <Badge variant="outline" className="border-slate-400">{book.category || "Uncategorized"}</Badge>
                <Badge variant="outline" className="border-slate-400">Book ID: {book.id}</Badge>
            </div>

            <div className="flex flex-col gap-1">
                <h2 className="text-xl font-semibold text-slate-600">Author</h2>
                <p>{book.author}</p>
            </div>


            <div className="flex flex-col gap-1">
                <h2 className="text-xl font-semibold text-slate-600">Description</h2>
                <p className="text-justify">{book.description || "None"}</p>
            </div>

            <Button variant="outline" onClick={() => addBook(book)} className="w-fit">
                <Plus /> Borrow book
            </Button>
        </div >
    )
}