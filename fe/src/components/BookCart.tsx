import { useAllowance } from "../hooks/useAllowance";
import { useCart } from "../hooks/useCart";
import { BookCard } from "./BookCard";
import { Button } from "./ui/button";

export const BookCart = () => {
    const { books, borrowBooks, isBorrowingBooks } = useCart();
    const { data: allowance } = useAllowance();

    const handleBorrow = () => borrowBooks();

    return (
        <div>
            <h2 className="text-xl text-center font-bold mb-4">I would like to borrow...</h2>

            {
                books.length === 0 ?
                    <p className="text-center">Hmm... it looks like you have nothing here.</p> :
                    <div className="flex flex-col gap-4">
                        {
                            books.map(book =>
                                <BookCard book={book} key={book.id} />
                            )
                        }

                        <div>
                            <Button variant="outline" className="block w-fit ml-auto" disabled={!allowance?.requestsAvailable || isBorrowingBooks} onClick={handleBorrow}>
                                Borrow {`(${books.length}/5)`}
                            </Button>
                            <p className="text-right mt-2">You have {allowance?.requestsAvailable ?? 0}/{allowance?.requestLimit ?? 0} request(s) left this month.</p>
                        </div>
                    </div>
            }
        </div>
    );
}

