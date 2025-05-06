import { Request } from "../types/types";
import { RequestStatusBadge } from "./RequestStatusBadge";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "./ui/table";

interface RequestTableProps {
    requests: Request[] | undefined;
    isLoading: boolean;
    isError: boolean;
}

export const RequestTable = ({ requests, isLoading, isError }: RequestTableProps) => {
    return (
        <Table>
            <TableHeader className="text-lg font-bold">
                <TableRow>
                    <TableHead className="text-center">ID</TableHead>
                    <TableHead className="text-center">Date Requested</TableHead>
                    <TableHead className="text-center">Books</TableHead>
                    <TableHead className="text-center">Status</TableHead>
                    <TableHead className="text-center">Approver</TableHead>
                </TableRow>
            </TableHeader>
            <TableBody>
                {
                    isLoading &&
                    <TableRow>
                        <TableCell colSpan={6} className="text-3xl text-center">Loading...</TableCell>
                    </TableRow>
                }
                {
                    isError &&
                    <TableRow>
                        <TableCell colSpan={6} className="text-3xl text-center">Error getting requests. Please try again later.</TableCell>
                    </TableRow>
                }
                {
                    requests &&
                    requests.map(request =>
                        <TableRow key={request.id}>
                            <TableCell className="text-center">{request.id}</TableCell>
                            <TableCell className="text-center">{request.dateRequested.toLocaleString()}</TableCell>
                            <TableCell className="text-wrap">
                                <ul className="flex flex-col gap-2">
                                    {
                                        request.books.map(book =>
                                            <li key={book.id} className="text-wrap">
                                                {book.title} by {book.author}
                                            </li>
                                        )
                                    }
                                </ul>
                            </TableCell>
                            <TableCell className="text-center">
                                <RequestStatusBadge status={request.status} />
                            </TableCell>
                            <TableCell className="text-center">
                                {request.approver ? `${request.approver.firstName} ${request.approver.lastName}` : "None"}
                            </TableCell>
                        </TableRow>
                    )
                }
            </TableBody>
        </Table>
    )
}