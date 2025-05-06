import { useQuery } from "@tanstack/react-query";
import { getMyBorrowings } from "../services/requestsServices";
import { PagedResponse, Request } from "../types/types";

export const useBorrowings = (pageNumber: number, pageSize: number) => {
    return useQuery<PagedResponse<Request>>({
        queryKey: ["my-borrowings", pageNumber, pageSize],
        queryFn: () => getMyBorrowings(pageNumber, pageSize)
    });
}