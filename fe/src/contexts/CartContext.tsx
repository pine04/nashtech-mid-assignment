import { ReactNode, useState, useEffect, createContext } from "react";
import { toast } from "sonner";
import { Book } from "../types/types";
import { UseMutateAsyncFunction, useMutation, useQueryClient } from "@tanstack/react-query";
import { makeBorrowingRequest } from "../services/requestsServices";
import { AxiosError } from "axios";
import { Request } from "../types/types";

interface CartContextType {
    books: Book[];
    addBook: (book: Book) => void;
    removeBook: (bookId: number) => void;
    clearAllBooks: () => void;
    borrowBooks: UseMutateAsyncFunction<Request, AxiosError<any, any>, void, unknown>;
    isBorrowingBooks: boolean;
}

export const CartContext = createContext<CartContextType | undefined>(undefined);

export const CartContextProvider = ({ children }: { children: ReactNode }) => {
    const [books, setBooks] = useState<Book[]>([]);
    const queryClient = useQueryClient();

    const updateBooks = (books: Book[]) => {
        setBooks(books);
        localStorage.setItem("selectedBooks", JSON.stringify(books));
    }

    const loadBooksFromLocalStorage = () => {
        const books = localStorage.getItem("selectedBooks");

        if (books) {
            setBooks(JSON.parse(books) as Book[]);
        }
    }

    const addBook = (book: Book) => {
        if (books.length === 5) {
            toast.error("You can only select up to 5 books per borrowing request.");

            return;
        }

        if (books.some(b => b.id === book.id)) {
            return;
        }

        updateBooks([...books, book]);
    }

    const removeBook = (bookId: number) => {
        updateBooks(books.filter(book => book.id !== bookId));
    }

    const clearAllBooks = () => {
        updateBooks([]);
    }

    const borrowBooksMutation = useMutation({
        mutationFn: () => {
            const bookIds = books.map(book => book.id);
            return makeBorrowingRequest({ bookIds });
        },
        onSuccess: () => {
            queryClient.invalidateQueries({
                predicate: (query) => query.queryKey.includes("my-borrowings")
            });
            clearAllBooks();
            toast.success("Borrowed books successfully. Check the My Borrowings tab to track your status.");
        },
        onError: (error: AxiosError<any, any>) => {
            toast.error(error.response?.data.detail || "An error happened while making the request. Please try again later.");
        }
    })

    useEffect(() => {
        loadBooksFromLocalStorage();
    }, []);

    const value = {
        books,
        addBook,
        removeBook,
        clearAllBooks,
        borrowBooks: borrowBooksMutation.mutateAsync,
        isBorrowingBooks: borrowBooksMutation.isPending,
    }

    return (
        <CartContext.Provider value={value}>
            {children}
        </CartContext.Provider>
    )
}