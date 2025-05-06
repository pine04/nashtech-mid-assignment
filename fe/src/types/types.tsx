export interface Category {
    id: number;
    name: string;
    bookCount: number;
}

export interface Book {
    id: number;
    title: string;
    author: string;
    description: string;
    category: string | null;
    categoryId: number | null;
    quantity: number;
    available: number;
}

export interface PagedResponse<T> {
    results: T[];
    totalRecordCount: number;
}

export interface User {
    id: number;
    firstName: string;
    lastName: string;
    email: string;
    role: "NormalUser" | "SuperUser";
}

export interface Allowance {
    requestsAvailable: number;
    requestLimit: number;
}

export interface Request {
    id: number;
    dateRequested: Date;
    status: string;
    requestor: {
        firstName: string,
        lastName: string,
        email: string,
    };
    approver: {
        firstName: string,
        lastName: string,
        email: string,
    } | null;
    books: {
        id: number;
        title: string;
        author: string;
        returned: boolean;
    }[];
}