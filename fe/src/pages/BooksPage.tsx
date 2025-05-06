import { useSearchParams } from "react-router-dom";
import { useBooks } from "../hooks/useBooks";
import { BookCart } from "../components/BookCart";
import { ALLOWED_PAGE_SIZES, toValidPageNumber, toValidPageSize } from "../lib/paginationUtils";
import { PageSizeSelect } from "../components/PageSizeSelect";
import { Pagination } from "../components/Pagination";
import { BookTable } from "../components/BookTable";
import { useAuth } from "../hooks/useAuth";

export const BooksPage = () => {
    const [searchParams, setSearchParams] = useSearchParams();

    const pageNumber = toValidPageNumber(searchParams.get("pageNumber"));
    const pageSize = toValidPageSize(searchParams.get("pageSize"));

    const { user } = useAuth();
    const { data, isLoading, isError } = useBooks(pageNumber, pageSize);

    const totalPageCount = data ? Math.ceil(data.totalRecordCount / pageSize) : 1;

    const handlePreviousPage = () => {
        if (pageNumber === 1) return;

        setSearchParams((prev) => {
            const newParams = new URLSearchParams(prev);
            newParams.set("pageNumber", `${pageNumber - 1}`);
            return newParams;
        });
    }

    const handleNextPage = () => {
        if (pageNumber === totalPageCount) return;

        setSearchParams((prev) => {
            const newParams = new URLSearchParams(prev);
            newParams.set("pageNumber", `${pageNumber + 1}`);
            return newParams;
        });
    }

    const handlePageSizeChange = (value: number) => {
        setSearchParams((prev) => {
            const newParams = new URLSearchParams(prev);
            newParams.set("pageNumber", "1");
            newParams.set("pageSize", value.toString());
            return newParams;
        });
    }

    const handleSetPageNumber = (value: number) => {
        if (value === pageNumber) return; // Skip if value is still the same.

        setSearchParams((prev) => {
            const newParams = new URLSearchParams(prev);
            newParams.set("pageNumber", `${value}`);
            return newParams;
        });
    }

    return (
        <div className="p-8">
            <h1 className="text-center text-3xl font-bold">Welcome {user?.firstName || ""} {user?.lastName || ""}!</h1>
            <p className="text-center text-xl text-slate-500">What would you like to read today?</p>

            <div className="mt-8 grid gap-8 md:gap-0 md:grid-cols-[2fr_1fr]">
                <div className="md:pr-8 md:border-r-2 md:border-r-slate-300 min-w-0">
                    <div className="flex flex-wrap items-center gap-x-8 gap-y-4 ml-auto w-fit mb-4 justify-center">
                        <PageSizeSelect
                            options={ALLOWED_PAGE_SIZES}
                            value={pageSize}
                            updateValue={handlePageSizeChange}
                        />

                        <Pagination
                            currentPageNumber={pageNumber}
                            maxPageNumber={totalPageCount}
                            goToPreviousPage={handlePreviousPage}
                            goToNextPage={handleNextPage}
                            setPageNumber={handleSetPageNumber}
                        />
                    </div>
                    <BookTable books={data?.results} isLoading={isLoading} isError={isError} />
                </div>

                <div className="order-first md:order-last md:sticky md:top-8 md:h-fit md:pl-8">
                    <BookCart />
                </div>
            </div>
        </div>
    )
}

