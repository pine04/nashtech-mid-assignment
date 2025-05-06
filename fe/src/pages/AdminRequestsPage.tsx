import { useSearchParams } from "react-router-dom";
import { ALLOWED_PAGE_SIZES, toValidPageNumber, toValidPageSize } from "../lib/paginationUtils";
import { PageSizeSelect } from "../components/PageSizeSelect";
import { Pagination } from "../components/Pagination";
import { AdminRequestTable } from "../components/AdminRequestTable";
import { useRequests } from "../hooks/useRequests";

export const AdminRequestsPage = () => {
    const [searchParams, setSearchParams] = useSearchParams();

    const pageNumber = toValidPageNumber(searchParams.get("pageNumber"));
    const pageSize = toValidPageSize(searchParams.get("pageSize"));

    const { data, isLoading, isError } = useRequests(pageNumber, pageSize);

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
            <h1 className="text-center text-3xl font-bold">Requests</h1>

            <div className="mt-8 w-full max-w-7xl mx-auto">
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

                <div className="grid">
                    <AdminRequestTable requests={data?.results} isLoading={isLoading} isError={isError} />
                </div>
            </div>
        </div>
    )
}