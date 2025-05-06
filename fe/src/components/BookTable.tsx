import { Plus } from "lucide-react";
import { Link } from "react-router-dom";
import { useCart } from "../hooks/useCart";
import { Book } from "../types/types";
import { Button } from "./ui/button";
import { Table, TableHeader, TableRow, TableHead, TableBody, TableCell } from "./ui/table";
import { Badge } from "./ui/badge";

interface BookTableProps {
    books: Book[] | undefined;
    isLoading: boolean;
    isError: boolean;
}

export const BookTable = ({ books, isLoading, isError }: BookTableProps) => {
    const { addBook } = useCart();

    return (
        <Table>
            <TableHeader className="text-lg font-bold">
                <TableRow>
                    <TableHead className="text-center">ID</TableHead>
                    <TableHead className="text-center w-48">Title</TableHead>
                    <TableHead className="text-center">Author</TableHead>
                    <TableHead className="text-center">Category</TableHead>
                    <TableHead className="text-center">Available</TableHead>
                    <TableHead className="text-center">Action</TableHead>
                </TableRow>
            </TableHeader>
            <TableBody>
                {
                    isLoading &&
                    <TableRow>
                        <TableCell colSpan={6} className="text-3xl text-center py-4">Loading...</TableCell>
                    </TableRow>
                }
                {
                    isError &&
                    <TableRow>
                        <TableCell colSpan={6} className="text-3xl text-center py-4">Error getting books. Please try again later.</TableCell>
                    </TableRow>
                }
                {
                    books &&
                    books.map(book =>
                        <TableRow key={book.id}>
                            <TableCell className="text-center">{book.id}</TableCell>
                            <TableCell>
                                <Link className="w-48 line-clamp-2 text-wrap underline" to={`/books/${book.id}`}>{book.title}</Link>
                            </TableCell>
                            <TableCell className="text-center">{book.author}</TableCell>
                            <TableCell className="text-center">{book.category ?? "Uncategorized"}</TableCell>
                            <TableCell className="text-center">
                                {
                                    book.available > 0 ?
                                        <Badge variant="outline" className="border-green-800 text-green-800">Available ({book.available})</Badge> :
                                        <Badge variant="outline" className="border-red-600 text-red-600">Not available</Badge>
                                }
                            </TableCell>
                            <TableCell className="text-center">
                                <Button variant="outline" size="icon" onClick={() => addBook(book)} disabled={book.available === 0}>
                                    <Plus />
                                </Button>
                            </TableCell>
                        </TableRow>
                    )
                }
            </TableBody>
        </Table>
    )
}