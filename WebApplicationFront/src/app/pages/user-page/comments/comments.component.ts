import {
  Component,
  HostListener,
  Input,
  OnDestroy,
  OnInit,
} from '@angular/core';
import { ScrollingModule } from '@angular/cdk/scrolling';
import { Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { UserCommentResponse } from '../../../data/interfaces/comment/user-comment-response.interface';
import { UserService } from '../../../service/user/user.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-comments',
  imports: [ScrollingModule, CommonModule],
  templateUrl: './comments.component.html',
  styleUrl: './comments.component.scss',
})
export class CommentsComponent implements OnInit, OnDestroy {
  public userName!: string;
  public comments: UserCommentResponse[] = [];
  public viewportHeight: string = '100vh';
  private unsubscribe$ = new Subject<void>();

  constructor(private router: Router, private userService: UserService) {
    this.userName =
      this.router.getCurrentNavigation()?.extras.state?.['userName'];
    console.log(this.userName);
  }

  public ngOnInit(): void {
    this.userService
      .getUserComments()
      .pipe(takeUntil(this.unsubscribe$))
      .subscribe({
        next: (val) => {
          this.comments = val;
        },
      });
    this.updateViewportHeight();
  }

  public ngOnDestroy(): void {
    this.unsubscribe$.next();
    this.unsubscribe$.complete();
  }

  @HostListener('window:resize', ['$event'])
  onResize(event: Event): void {
    this.updateViewportHeight();
  }

  private updateViewportHeight(): void {
    const filterHeaderHeight =
      document.querySelector('.header')?.clientHeight || 0;
    this.viewportHeight = `calc(100vh - var(--header-height) - ${filterHeaderHeight}px)`;
  }

  public goToProduct(productId: string): void {
    const selection = window.getSelection();
    if (selection && selection.toString().length > 0) {
      return;
    }
    this.router.navigate(['/product', productId]);
  }
}
