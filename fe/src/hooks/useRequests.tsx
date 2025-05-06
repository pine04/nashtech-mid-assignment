import { useQuery } from "@tanstack/react-query";
import { getRequests } from "../services/requestsServices";
import { PagedResponse, Request } from "../types/types";

export const useRequests = (pageNumber: number, pageSize: number) => {
    return useQuery<PagedResponse<Request>>({
        queryKey: ["requests", pageNumber, pageSize],
        queryFn: () => getRequests(pageNumber, pageSize)
    });
}