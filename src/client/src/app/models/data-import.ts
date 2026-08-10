export interface DataImport {
  id: number;
  uploaded: string;
  completed: string | null;
  status: string;
  validEmployees: number;
  invalidEmployees: number;
  validJobs: number;
  invalidJobs: number;
}
