import { Request } from "../types/types";
import { RequestStatusBadge } from "./RequestStatusBadge";
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
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "./ui/table";
import { Button } from "./ui/button";
import { useRejectRequest } from "../hooks/useRejectRequest";
import { useApproveRequest } from "../hooks/useApproveRequest";

interface ApproveRequestButtonProps {
    requestId: number;
    disabled: boolean;
}

const ApproveRequestButton = ({ requestId, disabled }: ApproveRequestButtonProps) => {
    const { approveRequest } = useApproveRequest(requestId);

    const handleApprove = () => {
        approveRequest();
    }

    return (
        <AlertDialog>
            <AlertDialogTrigger asChild>
                <Button variant="outline" disabled={disabled}>
                    Approve
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
                    <AlertDialogAction onClick={handleApprove}>Approve</AlertDialogAction>
                </AlertDialogFooter>
            </AlertDialogContent>
        </AlertDialog>
    )
}

interface RejectRequestButtonProps {
    requestId: number;
    disabled: boolean;
}

const RejectRequestButton = ({ requestId, disabled }: RejectRequestButtonProps) => {
    const { rejectRequest } = useRejectRequest(requestId);

    const handleReject = () => {
        rejectRequest();
    }

    return (
        <AlertDialog>
            <AlertDialogTrigger asChild>
                <Button variant="outline" disabled={disabled}>
                    Reject
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
                    <AlertDialogAction onClick={handleReject}>Reject</AlertDialogAction>
                </AlertDialogFooter>
            </AlertDialogContent>
        </AlertDialog>
    )
}

interface RequestTableProps {
    requests: Request[] | undefined;
    isLoading: boolean;
    isError: boolean;
}

export const AdminRequestTable = ({ requests, isLoading, isError }: RequestTableProps) => {
    return (
        <Table>
            <TableHeader className="text-lg font-bold">
                <TableRow>
                    <TableHead className="text-center">ID</TableHead>
                    <TableHead className="text-center">Date Requested</TableHead>
                    <TableHead className="text-center">Requestor</TableHead>
                    <TableHead className="text-center">Books</TableHead>
                    <TableHead className="text-center">Status</TableHead>
                    <TableHead className="text-center">Approver</TableHead>
                    <TableHead className="text-center">Action</TableHead>
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
                            <TableCell className="text-center">
                                {`${request.requestor.firstName} ${request.requestor.lastName}`}
                            </TableCell>
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
                            <TableCell>
                                <div className="flex items-center justify-center gap-4">
                                    <ApproveRequestButton requestId={request.id} disabled={request.status !== "Waiting"} />
                                    <RejectRequestButton requestId={request.id} disabled={request.status !== "Waiting"} />
                                </div>
                            </TableCell>
                        </TableRow>
                    )
                }
            </TableBody>
        </Table>
    )
}