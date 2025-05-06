import { Edit, Trash } from "lucide-react";
import { Link } from "react-router-dom";
import { Book } from "../types/types";
import { Button, buttonVariants } from "./ui/button";
import { Table, TableHeader, TableRow, TableHead, TableBody, TableCell } from "./ui/table";
import { Badge } from "./ui/badge";
import {
    AlertDialog,
    AlertDialogAction,
    AlertDialogCancel,
    AlertDialogContent,
    AlertDialogDescription,
    AlertDialogFooter,
    AlertDialogHeader,
    AlertDialogTitle,
    AlertDialogTrigger,
} from "@/components/ui/alert-dialog";
import { useDeleteBook } from "../hooks/useDeleteBook";

interface BookDeleteButtonProps {
    bookId: number;
}

const BookDeleteButton = ({ bookId }: BookDeleteButtonProps) => {
    const { deleteBook } = useDeleteBook(bookId);

    const handleDelete = () => {
        deleteBook();
    }

    return (
        <AlertDialog>
            <AlertDialogTrigger asChild>
                <Button variant="outline" size="icon">
                    <Trash />
                </Button>
            </AlertDialogTrigger>
            <AlertDialogContent>
                <AlertDialogHeader>
                    <AlertDialogTitle>Are you sure?</AlertDialogTitle>
                    <AlertDialogDescription>
                        This action cannot be undone.
                    </AlertDialogDescription>
                </AlertDialogHeader>
                <AlertDialogFooter>
                    <AlertDialogCancel>Cancel</AlertDialogCancel>
                    <AlertDialogAction className={buttonVariants({ variant: "destructive" })} onClick={handleDelete}>Continue</AlertDialogAction>
                </AlertDialogFooter>
            </AlertDialogContent>
        </AlertDialog>
    )
}

interface BookEditButtonProps {
    bookId: number;
}



const BookEditButton = ({ bookId }: BookEditButtonProps) => {
    return (
        <Button asChild variant="outline" size="icon">
            <Link to={`/admin/books/${bookId}/edit`}>
                <Edit />
            </Link>
        </Button>
    )
}

interface AdminBookTableProps {
    books: Book[] | undefined;
    isLoading: boolean;
    isError: boolean;
}

export const AdminBookTable = ({ books, isLoading, isError }: AdminBookTableProps) => {
    return (
        <Table>
            <TableHeader className="text-lg font-bold">
                <TableRow>
                    <TableHead className="text-center">ID</TableHead>
                    <TableHead className="text-center w-48">Title</TableHead>
                    <TableHead className="text-center">Author</TableHead>
                    <TableHead className="text-center">Category</TableHead>
                    <TableHead className="text-center">Quantity</TableHead>
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
                                <p className="w-48 line-clamp-2 text-wrap">{book.title}</p>
                            </TableCell>
                            <TableCell className="text-center">{book.author}</TableCell>
                            <TableCell className="text-center">{book.category ?? "Uncategorized"}</TableCell>
                            <TableCell className="text-center">{book.quantity}</TableCell>
                            <TableCell className="text-center">
                                {
                                    book.available > 0 ?
                                        <Badge variant="outline" className="border-green-800 text-green-800">Available ({book.available})</Badge> :
                                        <Badge variant="outline" className="border-red-600 text-red-600">Not available</Badge>
                                }
                            </TableCell>
                            <TableCell className="flex justify-center gap-4">
                                <BookEditButton bookId={book.id} />
                                <BookDeleteButton bookId={book.id} />
                            </TableCell>
                        </TableRow>
                    )
                }
            </TableBody>
        </Table>
    )
}